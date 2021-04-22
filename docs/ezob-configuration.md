# Ozob task definition

## Description

Ozob task definition file is used to define practical tasks for application execution. Using YAML as a configuration file is considered a human-readable data-serialization language. It is commonly used for configuration files and in applications where data is being stored or transmitted. 

## Usage

### Installing

To validate ozob file you will need to make sure that the following dependencies are installed on your system:
  - Visual studio code

###  Schema

Configuring a schema for ezob.yaml

1. Install the [Yaml](https://marketplace.visualstudio.com/items?itemName=redhat.vscode-yaml) extension.
2. Open VS Code's settings (`CTRL+,`)
3. Add a mapping for our schema.

Example:

```js
{
     "yaml.schemas": {
        "{project-address}\\src\\schema\\ezob-schema.json": "internet-ezob.yml"
    },
}
```

### Folder structure

Here's a folder structure for an ezob task definition:

```
src/     # Root directory.
|- samples/        #
    |- internet-ezob.yml
|- schema/         
    |- ezob-schema.json  # yml validation
    |- README.md  # setup instruction
```

### Setup generic data

Edit the *internet-ezob.yml* file to set configuration data:

```yml
name: Flex Onboarding (Global)

stages:
  - stage: CreateTickets
    displayName: Create tickets
    tasks:
      - task: CreateTicket
        ...
  - stage: InstallSoftwares
    displayName: Install Softwares
    dependsOn: CreateTickets
    tasks:
      - task: InstallSoftware
        ...

  - stage: SourceCodeSetup
    displayName: Source code setup
    dependsOn: InstallSoftwares
    tasks:
      - task: SourceCodeCheckout
        ...
```

You can find the above structure, we have two tags at the root level:
-   name: Flex Onboarding (Global)
-   stages: ...

The "name" tag is used for showing on application. The "stages" tag which is a list of stage, it includes three stage types:
    
    + CreateTickets
    + InstallSoftwares
    + SourceCodeSetup



### CreateTickets type

This type is in charge of tracking user ticket tasks.
Each stage contains a list of task.
  
For example: creating a ticket to request *VPN/Remote Access*

Example:
```yml
stages:
  - stage: CreateTickets
    displayName: Create tickets
    tasks:
      - task: CreateTicket
        displayName: Create ticket for VPN/Remote Access
        inputs:
          url: https://sample.com
          content: |
            Step 1: Open your browser, copy and paste the request link.
            Step 2: Login to access the request page.
            Step 3: Fulfill form as bellow
            - [blue](Open on behalf of this user)[/]: put your username
            - [blue](Is this request for a new user or an existing user)[/]: Existing User
            - [blue](Select the type of permission you need)[/]: Check both of (Remote Access) and (Sharepoint/RMS Knowledge Base/Release Notes)
            - [blue](Select Product)[/]: Select product that you participate (eg. Flex)
            - [blue](What Remote Access is Required?)[/]: put 'VPN and AWS environment'
            - [blue](Describe your request)[/]: put 'RMS General Permission for VPN, and AWS environment'
            - [blue](Email)[/]: put you email
            Step 4: Click on [blue](Submit)[/] button
```

### InstallSoftwares type
This type helps users installing required software/IDE quickly, One task InstallSoftware is used to install one software group. There is two kinds of installation like installing from exe/msi file (downloaded via assetAddress) or using choco for faster installation. Validation is used to verify that software is installed.

1. Installing from msi/exe:

    App would validate that you selected task executed by running "validation" unless app would download from "assetAddress", then execute "command"

2. Installing by Choco:

    You could find packages by searching by software name on [chocolatey packages webpage](https://chocolatey.org/), then add to "command".

Note: You should use silent mode to restrict user interaction, there are two type execution for command and validation, and you can choose one of following: 
    
    + cmd: using process file C:\Windows\System32\cmd.exe
    + pwsh: using process file C:\Program Files\PowerShell\7\pwsh.exe (recommended)

 ```path: .../stage/[tasks]/task/inputs/command/(cmd/pwsh)```
 
 ```path: .../stage/[tasks]/task/inputs/validation/(cmd/pwsh)```

Example:
```yml
  - stage: InstallSoftwares
    displayName: Install Softwares
    dependsOn: CreateTickets
    tasks:
      - task: InstallSoftware
        displayName: Install Powershell Core
        inputs:
          assetAddress: https://github.com/PowerShell/PowerShell/releases/download/v7.1.3/PowerShell-7.1.3-win-x64.msi
          downloadFilename: PowerShell-7.1.3-win-x64.msi
          autoInstallation: true
          command:
            cmd: start /wait msiexec.exe /package PowerShell-7.1.3-win-x64.msi /quiet ADD_EXPLORER_CONTEXT_MENU_OPENPOWERSHELL=1 ENABLE_PSREMOTING=1 REGISTER_MANIFEST=1
          validation:
            cmd: IF EXIST "C:\Program Files\PowerShell\7\pwsh.exe" echo ok

      - task: InstallSoftware
        displayName: Install Everything tool
        inputs:
          command: 
            pwsh: choco install everything --yes
          validation:
            pwsh: Test-Path 'C:\Program Files\Everything\Everything.exe'
```

### SourceCodeSetup type
 This type helps users cloning a repository using the command line.

Example:
```yml
  - stage: SourceCodeSetup
    displayName: Source code setup
    dependsOn: InstallSoftwares
    tasks:
      - task: SourceCodeCheckout
        displayName: Checkout source code
        inputs:
          username: $(username)
          password: $(password)
          checkoutLocation: $(checkoutLocation)
          sources:
            - https://github.com/libgit2/libgit2sharp.git
            - https://github.com/example/repo-name.git
```

 Username, Password, CheckoutLocation value will be replaced when you run ozob.exe with  --user-variables as following command line:
 
 ```
Ozob.exe run .\{ozob file folder address}\internet-ezob.yml --user-variables UserName=name@email.com Password=your_password CheckoutLocation=E:\\Projects
 ```
## References

- [chocolatey](https://chocolatey.org/)
- [JAML](https://yaml.org/)
- [JSON Schema](https://json-schema.org/)
- [Wikipedia: YML](https://en.wikipedia.org/wiki/YML)