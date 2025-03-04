# Adding Modules to the Module Library
Entries should follow this format:
```json
{
  "Name": "...",
  "FolderName": "...",
  "Description": "...",
  "Author": "...",
  "Categories": [],
  "Install": "..."
},
```
`Name`: Your module's display name (e.g.: "Syati Initializer Module")<br>
`FolderName`: Your module's folder name, should not contain spaces (e.g.: "Syati_Init")<br>
`Author`: The author(s) of your module.<br>
`Description`: Your module's description.<br>
`Categories`: Your module's categories. Try to use the same categories as other modules. If the existing categories do not fit your module, feel free to create a new one.<br>
`Install`: Where to download the module from. Check [Install Sources](https://github.com/SMGCommunity/SyatiManager/blob/main/Docs/Terminology.md#install-source).

# Adding Presets to the Preset Library
Entries should follow this format:
```json
{
  "Name": "...",
  "Description": "...",
  "Author": "...",
  "Install": "..."
},
```
`Name`: Your preset's display name (e.g.: "LiveActor Preset")<br>
`Description`: Your module's description.<br>
`Author`: The author(s) of your preset.<br>
`Install`: Where to download the preset from. Check [Install Sources](https://github.com/SMGCommunity/SyatiManager/blob/main/Docs/Terminology.md#install-source).