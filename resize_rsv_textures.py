from PIL import Image
import os

base_path = r"D:\project\stardew_valley\Ridgeside-Village-Mod\Ridgeside Development\[CP] Ridgeside Village\Assets\Objects"
output_path = r"D:\project\stardew_valley\StardewValleyMods\ControlTree\assets"

# ========== FruitTrees.png ==========
# 原始: 432x640
# 游戏读取格子: sprite_width=48, sprite_height=80, 共 9列 x 8行
# 行 = 树种 (TextureSpriteRow 0-7)
# 列布局:
#   Col 0-3 (x=0~191):   生长阶段，游戏读 Rectangle(col*48, row*80, 48, 80)，保持不变
#   Col 4-7 (x=192~383): 成熟树(春/夏/秋/冬)，游戏分两次读:
#       树体: Rectangle(col*48, row*80,    48, 64)  <- 需缩小
#       树桩: Rectangle(col*48, row*80+64, 48, 16)  <- 需缩小
#   Col 8   (x=384~431): 树冠顶部 overlay，保持不变
src_trees = Image.open(os.path.join(base_path, "FruitTrees.png"))
print(f"FruitTrees.png: {src_trees.width}x{src_trees.height}")

sw = 48   # 游戏实际 sprite 宽度
sh = 80   # 游戏实际 sprite 高度
body_h  = 64  # Cols 4-7: 树体高度
base_h  = 16  # Cols 4-7: 树干底部高度
stump_offset = 48  # Col 8: 树桩起始偏移 (y+48)
stump_h = 32  # Col 8: 树桩高度 (32px)
cols = 9
rows = 8
scale = 0.5

def resize_region(img, region_x, region_y, region_w, region_h, dst_canvas, anchor_bottom_y, scale):
    """从 img 裁剪区域，缩小后底部对齐、水平居中贴到 dst_canvas"""
    region = img.crop((region_x, region_y, region_x + region_w, region_y + region_h))
    bbox = region.getbbox()
    if not bbox:
        return
    cx0, cy0, cx1, cy1 = bbox
    content = region.crop((cx0, cy0, cx1, cy1))
    cw, ch = cx1 - cx0, cy1 - cy0
    sw_s = max(1, int(cw * scale))
    sh_s = max(1, int(ch * scale))
    scaled = content.resize((sw_s, sh_s), Image.Resampling.NEAREST)
    # 底部对齐，水平居中（在原始 region 宽度内居中）
    dst_y = anchor_bottom_y - sh_s
    dst_x = region_x + cx0 + (cw - sw_s) // 2
    dst_canvas.paste(scaled, (dst_x, dst_y), scaled)

trees_resized = Image.new("RGBA", (src_trees.width, src_trees.height), (0, 0, 0, 0))

for row in range(rows):
    for col in range(cols):
        src_x = col * sw
        src_y = row * sh

        if col in [4, 5, 6, 7]:
            # 把整个 80px 格子作为一个整体缩小，底部对齐
            # 这样树干和树干底之间不会产生空隙，游戏分两次读 (h=64, h=16) 自然衔接
            cell = src_trees.crop((src_x, src_y, src_x + sw, src_y + sh))
            bbox = cell.getbbox()
            if bbox:
                cx0, cy0, cx1, cy1 = bbox
                content = cell.crop((cx0, cy0, cx1, cy1))
                cw, ch = cx1 - cx0, cy1 - cy0
                sw_s = max(1, int(cw * scale))
                sh_s = max(1, int(ch * scale))
                scaled = content.resize((sw_s, sh_s), Image.Resampling.NEAREST)
                # 底部对齐，水平居中
                dst_y = src_y + sh - sh_s
                dst_x = src_x + cx0 + (cw - sw_s) // 2
                trees_resized.paste(scaled, (dst_x, dst_y), scaled)

        elif col == 8:
            # Col 8 上方 48px: 树冠/果实图标，原样复制
            canopy = src_trees.crop((src_x, src_y, src_x + sw, src_y + stump_offset))
            trees_resized.paste(canopy, (src_x, src_y), canopy)
            # 树桩 (32px, 从 y+48 开始): 缩小，锚点 = cell 底部 (src_y + sh)
            resize_region(src_trees, src_x, src_y + stump_offset, sw, stump_h,
                          trees_resized, src_y + sh, scale)

        else:
            # 生长阶段 (col 0-3): 原样复制
            sprite = src_trees.crop((src_x, src_y, src_x + sw, src_y + sh))
            trees_resized.paste(sprite, (src_x, src_y), sprite)

output_trees = os.path.join(output_path, "fruit_trees_resized.png")
trees_resized.save(output_trees)
print(f"已保存: {output_trees} ({trees_resized.width}x{trees_resized.height})")
print("完成!")
