# WpfBlazor - K0x Workbench

This is a hybrid WPF-Blazor application that combines the power of WPF for native Windows desktop functionality with Blazor for modern web UI components.

The K0x Workbench application serves as a developer workbench tool.

This README focuses on the techincal details of the application to provide an understanding of how it works.

## Architecture Overview

The application uses:
- **WPF** for the main window and native Windows integration
- **Blazor WebView** for rendering modern web-based UI components within the main window
- **Microsoft WebView2** runtime for hosting Blazor content
- **Dependency Injection** with Microsoft.Extensions for service management
- **Serilog** for structured logging
- **ASP.NET Core Components** for Blazor functionality

## Application Startup Sequence

The following sequence describes how the WpfBlazor app starts up, configures itself, and displays the initial Blazor page in the BlazorWebView:

### 1. Entry Point (`Program.Main`)
- **Entry**: Application starts at `Program.Main()` method
- **Thread Model**: Marked with `[STAThread]` for WPF compatibility
- **Error Handling**: Wrapped in try-catch with MessageBox fallback for configuration errors

### 2. Configuration and Dependency Injection Setup
- **Method**: `ProgramConfiguration.Setup()`
- **Host Builder**: Creates default host builder with standard .NET configuration
- **Configuration Sources**:
  - `appsettings.json` (loaded by default)
  - Environment variables (standard)
  - Environment variables with `K0xWorkbench_` prefix (custom override)
- **Service Registration**:
  - Bench JSON file services
  - App title service
  - Data folder path provider
  - Recent benches JSON file services
  - WPF Blazor WebView services
  - MainWindow (transient)
- **Logging**: Configures Serilog with file and console sinks
- **Configuration Reload**: Sets up change token handlers for live configuration updates

### 3. Command Line Argument Processing
- **Method**: `InitBenchFilePathProviderFromArgs()`
- **Purpose**: Processes command line arguments for bench file path
- **Behavior**: Sets the file path in the singleton `IBenchFilePathProvider` service

### 4. WPF Application Initialization
- **Method**: `RunWpfApp()`
- **App Creation**: Instantiates `App` class with the configured `IServiceProvider`
- **Component Initialization**: Calls `app.InitializeComponent()` to load XAML resources
- **Window Resolution**: Resolves `MainWindow` from DI container
- **Error Handling**: Special handling for WebView2 runtime missing errors

### 5. MainWindow Setup
- **Constructor**: `MainWindow(IAppTitleGetService appTitleGetService)`
- **XAML Loading**: Calls `InitializeComponent()` to load MainWindow.xaml
- **BlazorWebView Configuration**:
  - Sets `blazorWebView.Services` to the application's `IServiceProvider`
  - This enables dependency injection within Blazor components
- **Title Management**: Wires up app title change handlers

### 6. BlazorWebView Initialization
- **Host Page**: Points to `wwwroot/index.html`
- **Root Component**: Configures `BlazorApp` component with selector `#app`
- **Service Provider**: Uses the WPF app's service provider for DI

### 7. Blazor App Startup
- **Root Component**: `BlazorApp.razor` serves as the application root
- **Router Configuration**:
  - Uses `Router` component with assembly scanning
  - Default layout: `MainLayout`
  - Focus management with `FocusOnNavigate`
- **Error Handling**: Displays "Not found" page for unmatched routes

### 8. Initial Page Rendering
- **HTML Host**: `wwwroot/index.html` provides the base HTML structure
- **Loading State**: Shows "Loading..." while Blazor initializes
- **CSS Resources**: Loads Bootstrap, Bootstrap Icons, and custom stylesheets
- **JavaScript**: Loads `blazor.webview.js` for WebView2 integration
- **App Mount**: Blazor app mounts to the `<div id="app">` element

### 9. Navigation and Layout
- **Main Layout**: `MainLayout.razor` provides the overall page structure
- **Navigation**: `NavMenu.razor` handles application navigation
- **Default Route**: Typically resolves to `Home.razor` or similar default page
- **Page Types**: Includes pages like `BenchPage`, `RecentPage`, and `Home`

## Key Components

### Core Files
- `Program.cs` - Application entry point and startup orchestration
- `ProgramConfiguration.cs` - Dependency injection and configuration setup
- `App.xaml.cs` - WPF Application class with service provider
- `MainWindow.xaml` - Main WPF window containing BlazorWebView
- `MainWindow.xaml.cs` - Code-behind for main window setup

### Blazor Components
- `BlazorApp.razor` - Root Blazor component with routing
- `MainLayout.razor` - Primary layout component
- `NavMenu.razor` - Navigation menu component
- `wwwroot/index.html` - HTML host page for Blazor content

### Pages
- `Home.razor` - Default home page
- `BenchPage.razor` - Workbench/bench management page
- `RecentPage.razor` - Recent files/projects page

## Dependencies

### Key NuGet Packages
- `Microsoft.AspNetCore.Components.WebView.Wpf` - Blazor WebView for WPF
- `Microsoft.AspNetCore.Components.Web` - Blazor web components
- `Microsoft.Extensions.Hosting` - Generic host and DI container
- `Serilog.*` - Structured logging packages

### Runtime Requirements
- .NET 9.0 Windows
- Microsoft Edge WebView2 Runtime (automatically prompts for installation if missing. The user must take action outside of the application to install the missing runtime)

## Configuration

The application supports configuration through:
- `appsettings.json` - Default configuration file
- Environment variables (standard .NET behavior)
- Environment variables with `K0xWorkbench_` prefix (for overrides)
- Command line arguments (for bench file path)

Configuration changes are detected at runtime and trigger reload notifications.

## Error Handling

The application includes comprehensive error handling:
- Configuration setup errors show MessageBox dialogs
- Missing WebView2 runtime detection with download instructions
- Unhandled exceptions are logged and displayed to the user
- Blazor error UI for client-side errors
