# 许可证密钥生成器 - Android 版本

这是使用 .NET MAUI 开发的 Android 应用程序，功能与原 Windows WPF 版本完全一致。

## 功能特性

- 输入设备序列号生成许可证密钥
- 盐值管理（添加、编辑、删除）
- 支持永久授权和限时授权
- 一键复制生成的密钥
- 显示调试信息
- 数据本地持久化存储

---

## ⚠️ 常见问题解决

### 问题：提示缺少 maui-android 工作负载

如果看到类似这样的错误：
```
error NETSDK1147: 要构建此项目，必须安装以下工作负载: maui-android
```

**解决方法（任选其一）：**

```bash
# 方法 1：自动恢复项目所需的工作负载（推荐）
dotnet workload restore

# 方法 2：直接安装 MAUI Android 工作负载
dotnet workload install maui-android
```

或者直接使用我们的自动化构建脚本，它会自动处理这个问题！

---

## 🚀 构建方法（无需 Visual Studio）

### 方法一：使用自动化构建脚本（推荐）

#### Windows 用户

双击运行 `build.ps1` 或在 PowerShell 中执行：

```powershell
cd LicenseKeyGenerator.Maui
.\build.ps1
```

#### Linux/macOS 用户

在终端中执行：

```bash
cd LicenseKeyGenerator.Maui
chmod +x build.sh  # 首次运行需要添加执行权限
./build.sh
```

---

### 方法二：使用 .NET CLI 命令行

#### 环境要求

仅需要安装：
- **.NET 8.0 SDK** (无需 Visual Studio)
- Android SDK (API 21 或更高)

下载地址：https://dotnet.microsoft.com/download/dotnet/8.0

#### 构建步骤

```bash
# 1. 进入项目目录
cd LicenseKeyGenerator.Maui

# 2. 恢复 NuGet 包
dotnet restore

# 3. 安装 MAUI Android 工作负载
dotnet workload install maui-android

# 4. 构建 Release 版本并生成 APK
dotnet publish -c Release -f net8.0-android -p:AndroidPackageFormat=apk -p:AndroidKeyStore=False
```

#### 生成的 APK 位置

```
bin/Release/net8.0-android/publish/com.licensekeygenerator.app.apk
```

---

### 方法三：使用 GitHub Actions 自动构建（最简单）

如果你将代码推送到 GitHub 仓库，可以使用 GitHub Actions 自动构建：

#### 步骤：

1. 将代码推送到 GitHub 仓库
2. 进入仓库的 **Actions** 标签页
3. 选择 **Build Android APK** 工作流
4. 点击 **Run workflow**
5. 填写版本号，选择构建类型（Release/Debug）
6. 等待构建完成（约 5-10 分钟）
7. 在构建结果中下载生成的 APK 文件

#### 优势：

- ✅ 无需配置任何本地开发环境
- ✅ 每次提交自动构建
- ✅ 可以手动触发构建
- ✅ 自动保存 APK 构件 30 天

---

## 📦 使用 Visual Studio 构建（可选）

如果你有 Visual Studio 2022，也可以使用它：

### 环境要求
- Visual Studio 2022 (17.8 或更高版本)
- "使用 .NET 的移动开发" 工作负载

### 构建步骤
1. 在 Visual Studio 中打开解决方案
2. 选择 "Android" 作为目标平台
3. 选择 Release 配置
4. 点击 "生成" -> "发布所选内容"
5. 按照向导生成签名的 APK

---

## 🔧 项目结构

```
LicenseKeyGenerator.Maui/
├── Models/                    # 数据模型
│   └── SaltItem.cs           # 盐值项
├── Services/                  # 服务层
│   ├── LicenseService.cs     # 密钥生成服务
│   └── ConfigurationService.cs # 配置存储服务
├── Converters/                # 值转换器
│   └── NotEmptyStringToBoolConverter.cs
├── Platforms/Android/        # Android 特定代码
│   ├── MainActivity.cs
│   └── MainApplication.cs
├── Resources/                # 资源文件
│   ├── AppIcon/             # 应用图标
│   └── Splash/              # 启动画面
├── MainPage.xaml            # 主界面
├── MainPage.xaml.cs         # 主界面代码
├── App.xaml                 # 应用程序
├── App.xaml.cs
├── MauiProgram.cs           # 应用入口
├── build.ps1                # Windows 构建脚本
├── build.sh                 # Linux/macOS 构建脚本
└── LicenseKeyGenerator.Maui.csproj
```

---

## 与原版本的差异

- 使用 .NET MAUI 替代 WPF
- 使用 MAUI 的 CollectionView 替代 WPF 的 ListBox
- 使用 MAUI 的 DisplayAlert 替代 MessageBox
- 使用 MAUI 的 Clipboard API 替代 Windows 剪贴板
- 使用 FileSystem.AppDataDirectory 进行数据存储
- 完全保留了原有的密钥生成算法

---

## 签名配置

如需对 APK 进行签名，请在项目文件中添加：

```xml
<PropertyGroup Condition="'$(Configuration)'=='Release'">
  <AndroidKeyStore>True</AndroidKeyStore>
  <AndroidSigningStoreFile>your.keystore</AndroidSigningStoreFile>
  <AndroidSigningStorePass>your_store_password</AndroidSigningStorePass>
  <AndroidSigningKeyAlias>your_alias</AndroidSigningKeyAlias>
  <AndroidSigningKeyPass>your_key_password</AndroidSigningKeyPass>
</PropertyGroup>
```

---

## 许可证

与原项目相同的许可证。
