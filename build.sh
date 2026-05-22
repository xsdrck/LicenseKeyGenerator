#!/bin/bash

# 许可证密钥生成器 - Android 构建脚本 (Bash)
# 适用于 Linux 和 macOS

set -e

echo "========================================"
echo "  许可证密钥生成器 - Android 构建"
echo "========================================"
echo ""

# 检查 .NET SDK
echo "[1/5] 检查 .NET SDK..."
if ! command -v dotnet &> /dev/null; then
    echo "✗ 未找到 .NET SDK，请先安装 .NET 8.0 SDK"
    echo "下载地址: https://dotnet.microsoft.com/download/dotnet/8.0"
    exit 1
fi

DOTNET_VERSION=$(dotnet --version)
echo "✓ .NET SDK 版本: $DOTNET_VERSION"

# 恢复 NuGet 包
echo ""
echo "[2/6] 恢复 NuGet 包..."
dotnet restore
echo "✓ 包恢复成功"

# 安装/恢复 MAUI 工作负载
echo ""
echo "[3/6] 检查并安装 MAUI Android 工作负载..."
if ! dotnet workload list | grep -q "maui-android"; then
  echo "maui-android 工作负载未安装，正在安装..."
  if ! dotnet workload install maui-android; then
    echo "✗ 工作负载安装失败，尝试使用 workload restore..."
    if ! dotnet workload restore; then
      echo "✗ 工作负载安装/恢复失败，请手动运行: dotnet workload install maui-android"
      exit 1
    fi
  fi
  echo "✓ maui-android 工作负载安装成功"
else
  echo "✓ maui-android 工作负载已安装"
fi

# 构建 Debug 版本（可选）
echo ""
read -p "是否构建 Debug 版本？(y/n) [默认: n]: " build_debug
if [[ "$build_debug" == "y" || "$build_debug" == "Y" ]]; then
  echo "[4/6] 构建 Debug 版本..."
  dotnet build -f net8.0-android -c Debug
  echo "✓ Debug 构建成功"
fi

# 构建 Release 版本
echo ""
echo "[5/6] 构建 Release 版本并生成 APK..."
dotnet publish -c Release -f net8.0-android -p:AndroidPackageFormat=apk -p:AndroidKeyStore=False
echo "✓ Release 构建成功"

# 查找生成的 APK
echo ""
echo "[6/6] 查找生成的 APK..."
APK_PATH=$(find bin/Release/net8.0-android/publish -name "*.apk" -type f 2>/dev/null | head -1)

if [[ -f "$APK_PATH" ]]; then
    echo "✓ APK 生成成功！"
    echo ""
    echo "APK 文件位置:"
    echo "$APK_PATH"
    echo ""
    FILE_SIZE=$(du -h "$APK_PATH" | cut -f1)
    echo "文件大小: $FILE_SIZE"
else
    echo "✗ 未找到生成的 APK 文件"
    echo "请检查 bin/Release/net8.0-android/publish 目录"
fi

echo ""
echo "========================================"
echo "  构建完成！"
echo "========================================"
