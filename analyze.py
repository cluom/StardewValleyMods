from PIL import Image
import os

base_path = r"D:\project\stardew_valley\Ridgeside-Village-Mod\Ridgeside Development\[CP] Ridgeside Village\Assets\Objects"

src = Image.open(os.path.join(base_path, "FruitTrees.png"))
print(f"FruitTrees.png: {src.width}x{src.height}")

# 分析每行/每列的像素分布
# 检查是否有透明分隔线或明显的边界
width, height = src.width, src.height

# 尝试检测列边界
print("\n检测列边界...")
for col in range(width):
    # 检查这一列是否全是透明
    col_data = [src.getpixel((col, y))[3] for y in range(height)]
    if all(p == 0 for p in col_data):
        print(f"  列 {col} 是透明分隔线")

# 尝试检测行边界
print("\n检测行边界...")
for row in range(height):
    row_data = [src.getpixel((x, row))[3] for x in range(width)]
    if all(p == 0 for p in row_data):
        print(f"  行 {row} 是透明分隔线")

# 分析非透明区域
print("\n分析非透明区域...")
top_row = -1
bottom_row = -1
for row in range(height):
    row_data = [src.getpixel((x, row))[3] for x in range(width)]
    if any(p > 0 for p in row_data):
        if top_row == -1:
            top_row = row
        bottom_row = row

print(f"非透明区域: 行 {top_row} 到 {bottom_row} (高度 {bottom_row - top_row})")

# 检查列
left_col = -1
right_col = -1
for col in range(width):
    col_data = [src.getpixel((col, y))[3] for y in range(height)]
    if any(p > 0 for p in col_data):
        if left_col == -1:
            left_col = col
        right_col = col

print(f"非透明区域: 列 {left_col} 到 {right_col} (宽度 {right_col - left_col})")

# 如果是8行8列的布局
# 432 / ? = ?
# 640 / 8 = 80
# 432 / 8 = 54
# 假设8行8列: 每格54x80
print(f"\n如果8行: {height/8}px/行")
print(f"如果8列: {width/8}px/列")
print(f"如果4列: {width/4}px/列")
