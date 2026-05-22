# 许可证密钥生成器 - Android 构建脚本 (PowerShell)
# 适用于 Windows

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  许可证密钥生成器 - Android 构建" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# 检查 .NET SDK
Write-Host "[1/5] 检查 .NET SDK..." -ForegroundColor Yellow
try {
    $dotnetVersion = dotnet --version
    Write-Host "✓ .NET SDK 版本: $dotnetVersion" -ForegroundColor Green
} catch {
    Write-Host "✗ 未找到 .NET SDK，请先安装 .NET 8.0 SDK" -ForegroundColor Red
    Write-Host "下载地址: https://dotnet.microsoft.com/download/dotnet/8.0" -ForegroundColor Yellow
    exit 1
}

# 恢复 NuGet 包
Write-Host ""
Write-Host "[2/6] 恢复 NuGet 包..." -ForegroundColor Yellow
dotnet restore
if ($LASTEXITCODE -ne 0) {
  Write-Host "✗ 恢复包失败" -ForegroundColor Red
  exit 1
}
Write-Host "✓ 包恢复成功" -ForegroundColor Green

# 安装/恢复 MAUI 工作负载
Write-Host ""
Write-Host "[3/6] 检查并安装 MAUI Android 工作负载..." -ForegroundColor Yellow
try {
  $workloads = dotnet workload list | Select-String "maui-android"
  if (-not $workloads) {
    Write-Host "maui-android 工作负载未安装，正在安装..." -ForegroundColor Yellow
    dotnet workload install maui-android
    if ($LASTEXITCODE -ne 0) {
      Write-Host "✗ 工作负载安装失败，尝试使用 workload restore..." -ForegroundColor Yellow
      dotnet workload restore
      if ($LASTEXITCODE -ne 0) {
        Write-Host "✗ 工作负载安装/恢复失败，请手动运行: dotnet workload install maui-android" -ForegroundColor Red
        exit 1
      }
    }
    Write-Host "✓ maui-android 工作负载安装成功" -ForegroundColor Green
  } else {
    Write-Host "✓ maui-android 工作负载已安装" -ForegroundColor Green
  }
} catch {
  Write-Host "警告: 检查工作负载时出错，继续构建..." -ForegroundColor Yellow
}

# 构建 Debug 版本（可选）
Write-Host ""
$buildDebug = Read-Host "是否构建 Debug 版本？(y/n) [默认: n]"
if ($buildDebug -eq 'y' -or $buildDebug -eq 'Y') {
  Write-Host "[4/6] 构建 Debug 版本..." -ForegroundColor Yellow
  dotnet build -f net8.0-android -c Debug
  if ($LASTEXITCODE -ne 0) {
    Write-Host "✗ Debug 构建失败" -ForegroundColor Red
    exit 1
  }
  Write-Host "✓ Debug 构建成功" -ForegroundColor Green
}

# 构建 Release 版本
Write-Host ""
Write-Host "[5/6] 构建 Release 版本并生成 APK..." -ForegroundColor Yellow
dotnet publish -c Release -f net8.0-android -p:AndroidPackageFormat=apk -p:AndroidKeyStore=False
if ($LASTEXITCODE -ne 0) {
  Write-Host "✗ Release 构建失败" -ForegroundColor Red
  exit 1
}
Write-Host "✓ Release 构建成功" -ForegroundColor Green

# 查找生成的 APK
Write-Host ""
Write-Host "[6/6] 查找生成的 APK..." -ForegroundColor Yellow
$apkPath = Get-ChildItem -Path "bin/Release/net8.0-android/publish" -Filter "*.apk" -Recurse | Select-Object -First 1
if ($apkPath) {
    Write-Host "✓ APK 生成成功！" -ForegroundColor Green
    Write-Host ""
    Write-Host "APK 文件位置:" -ForegroundColor Cyan
    Write-Host $apkPath.FullName -ForegroundColor White
    Write-Host ""
    Write-Host "文件大小: $([math]::Round($apkPath.Length / 1MB, 2)) MB" -ForegroundColor Gray
    
    # 询问是否打开文件夹
    $openFolder = Read-Host "是否打开 APK 所在文件夹？(y/n) [默认: y]"
    if ($openFolder -ne 'n' -and $openFolder -ne 'N') {
        explorer.exe $apkPath.DirectoryName
    }
} else {
    Write-Host "✗ 未找到生成的 APK 文件" -ForegroundColor Red
    Write-Host "请检查 bin/Release/net8.0-android/publish 目录" -ForegroundColor Yellow
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  构建完成！" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Cyan
