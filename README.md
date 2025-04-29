# XML Configuration Handler for .NET Framework 4.8

## Description

This is a simple yet flexible C# class designed to handle application configuration settings by storing them in an XML file. It serves as a modern and type-aware alternative to traditional INI file parsing for .NET Framework 4.8 applications, and includes `INotifyPropertyChanged` support which can be particularly useful in WPF projects for data binding scenarios.

## Features

* **XML-Based Storage:** Persists settings in a human-readable XML format.
* **INI Alternative:** Offers a structured way to manage configurations compared to simple key-value pairs in INI files.
* **Type Handling:** Provides convenient methods for reading and writing common data types (String, Double, Integer, Boolean).
* **Dynamic Settings:** Utilizes `ExpandoObject` internally, allowing dynamic access and management of settings.
* **Load/Save:** Easy methods to load settings from and save them to the specified file path.
* **Setting Management:** Functions to add, remove, and check for the existence of settings.
* **WPF Friendly:** Implements `INotifyPropertyChanged` to notify listeners (like UI elements) when the `Settings` object is replaced.

## How to Use

1.  **Instantiate the Handler:**
    Create an instance specifying the path to your configuration file.
    ```csharp
    string configFilePath = "config.xml"; // Replace with your desired file path
    XmlConfigurationHandler config = new XmlConfigurationHandler(configFilePath);
    ```

2.  **Define Default Settings (Recommended):**
    Use `AddSetting` to define the expected keys and their default string values. This is useful for initializing settings when the file doesn't exist or ensuring keys are present. If a key already exists (either from a previous `AddSetting` call or loaded from file), `AddSetting` will add/update its value.
    ```csharp
    config.AddSetting("WindowWidth", "800");
    config.AddSetting("WindowHeight", "600");
    config.AddSetting("DatabasePath", "Data/app.db");
    config.AddSetting("EnableFeatureX", "true");
    config.AddSetting("AnimationDurationSeconds", "0.5");
    ```

3.  **Load Settings from File:**
    Call `LoadFile()` to load settings from the specified path. If the file doesn't exist, `LoadFile()` will implicitly call `SaveFile()` to create it based on the settings currently in the handler (including any defaults you added).
    ```csharp
    config.LoadFile();
    ```

4.  **Read Settings:**
    Use the `ReadString`, `ReadDouble`, `ReadInt`, and `ReadBoolean` methods to retrieve settings in the desired data type. These methods handle basic parsing and return default/NaN/null on failure or if the key is not found.
    ```csharp
    int windowWidth = config.ReadInt("WindowWidth");
    bool featureEnabled = config.ReadBoolean("EnableFeatureX");
    double duration = config.ReadDouble("AnimationDurationSeconds");
    double roundedDuration = config.ReadDouble("AnimationDurationSeconds", 1); // Reads and rounds to 1 decimal place
    string dbPath = config.ReadString("DatabasePath");
    ```

5.  **Write Settings:**
    Use the `WriteString`, `WriteDouble`, `WriteInt`, and `WriteBoolean` methods to update setting values. Note that `WriteString` **only** writes to keys that already exist in the handler's settings collection.
    ```csharp
    config.WriteInt("WindowWidth", 1024);
    config.WriteBoolean("EnableFeatureX", false);
    config.WriteDouble("AnimationDurationSeconds", 0.75, 2); // Writes 0.75, optionally rounded to 2 decimal places
    config.WriteString("DatabasePath", "C:/NewData/app.db");

    // config.WriteString("NewKey", "NewValue"); // This would *not* work as "NewKey" wasn't added or loaded. Use AddSetting first.
    ```

6.  **Save Settings to File:**
    Persist the current settings to the XML file. This will overwrite the existing file.
    ```csharp
    config.SaveFile();
    ```

7.  **Dynamic Access (Advanced):**
    You can access the underlying `ExpandoObject` directly via the `Settings` property for more dynamic operations, but be mindful of type safety.
    ```csharp
    dynamic settings = config.Settings;
    // Read (returns object, might need casting)
    string rawWidth = settings.WindowWidth;
    // Write (adds/updates directly)
    settings.NewDynamicProperty = "This was added dynamically";
    ```

8.  **WPF Data Binding (Considerations):**
    The `Settings` property is an `ExpandoObject` and the class implements `INotifyPropertyChanged`. You can bind directly to properties added to `Settings` (e.g., `<TextBox Text="{Binding Settings.MyBoundSetting}" />`). However, updates made *directly* to the `ExpandoObject` might not trigger `PropertyChanged` notifications on *specific properties* in the same way strongly-typed objects do. The `PropertyChanged` event is raised when the entire `Settings` object *instance* is replaced (which happens in the `Settings` setter), but not necessarily for individual property changes within the ExpandoObject via `Read/Write` methods or dynamic access.

## Requirements

* .NET Framework 4.8

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Contributing

Contributions, bug reports, and feature suggestions are welcome!

If you encounter any issues or have ideas for improvement, please feel free to open an [Issue](https://github.com/argerlee/XmlConfigurationHandler/issues) on the GitHub repository page.

If you'd like to contribute code, please feel free to fork the repository and submit a [Pull Request](https://github.com/argerlee/XmlConfigurationHandler/pulls).

## Contact

You can reach me via my GitHub profile: [@argerlee](https://github.com/argerlee)
