from PIL import Image
import os

def shrink_treetop_and_stump(src_path, dst_path, scale=0.5):
    """缩小野生树的TreeTop和Stump部分"""
    src = Image.open(src_path)
    print(f"Source: {src.width}x{src.height}")

    # 复制原图作为基础
    dst = src.copy()

    treetop_w, treetop_h = 48, 96
    treetop_x, treetop_y = 0, 0
    stump_w, stump_h = 16, 32
    stump_x, stump_y = 32, 96

    for row in range(src.height // 128):
        row_offset = row * 128

        # ===== 处理TreeTop =====
        # 挖掉TreeTop区域
        dst.paste(Image.new("RGBA", (treetop_w, treetop_h), (0, 0, 0, 0)),
                 (treetop_x, row_offset + treetop_y))

        # 从原图提取TreeTop并缩小
        treetop_region = src.crop((treetop_x, row_offset + treetop_y,
                                    treetop_x + treetop_w, row_offset + treetop_y + treetop_h))
        bbox = treetop_region.getbbox()
        if bbox:
            cx0, cy0, cx1, cy1 = bbox
            content = treetop_region.crop((cx0, cy0, cx1, cy1))
            cw, ch = cx1 - cx0, cy1 - cy0
            sw_s = max(1, int(cw * scale))
            sh_s = max(1, int(ch * scale))
            scaled = content.resize((sw_s, sh_s), Image.Resampling.NEAREST)
            # 底部对齐
            dst_y = row_offset + treetop_y + treetop_h - sh_s
            dst_x = treetop_x + cx0 + (cw - sw_s) // 2
            dst.paste(scaled, (dst_x, dst_y), scaled)

        # ===== 处理Stump =====
        # 挖掉Stump区域
        dst.paste(Image.new("RGBA", (stump_w, stump_h), (0, 0, 0, 0)),
                 (stump_x, row_offset + stump_y))

        # 从原图提取Stump并缩小
        stump_region = src.crop((stump_x, row_offset + stump_y,
                                  stump_x + stump_w, row_offset + stump_y + stump_h))
        bbox = stump_region.getbbox()
        if bbox:
            cx0, cy0, cx1, cy1 = bbox
            content = stump_region.crop((cx0, cy0, cx1, cy1))
            cw, ch = cx1 - cx0, cy1 - cy0
            sw_s = max(1, int(cw * scale))
            sh_s = max(1, int(ch * scale))
            scaled = content.resize((sw_s, sh_s), Image.Resampling.NEAREST)
            # 底部对齐
            dst_y = row_offset + stump_y + stump_h - sh_s
            dst_x = stump_x + cx0 + (cw - sw_s) // 2
            dst.paste(scaled, (dst_x, dst_y), scaled)

    dst.save(dst_path)
    print(f"Saved: {dst_path}")


src_dir = r"D:/project/stardew_valley/StardewValleyExpanded/Stardew Valley Expanded/[CP] Stardew Valley Expanded/assets/Trees"
assets_dir = r"D:/project/stardew_valley/StardewValleyMods/ControlTree/assets"

trees = [
    "spring_Birch_Tree.png",
    "summer_Birch_Tree.png",
    "fall_Birch_Tree.png",
    "winter_Birch_Tree.png",
    "spring_Fir_Tree.png",
    "summer_Fir_Tree.png",
    "fall_Fir_Tree.png",
    "winter_Fir_Tree.png",
]

for filename in trees:
    src_path = os.path.join(src_dir, filename)
    if not os.path.exists(src_path):
        print(f"Not found: {src_path}")
        continue

    dst_path = os.path.join(assets_dir, filename)
    shrink_treetop_and_stump(src_path, dst_path, scale=0.5)
    print(f"Generated: {dst_path}")

print("\nDone! All SVE wild tree textures with shrunk TreeTop and Stump generated.")