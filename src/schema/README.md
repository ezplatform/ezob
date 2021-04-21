# Schema

Configuring a schema for ezob.yaml

1. Install the [Yaml](https://marketplace.visualstudio.com/items?itemName=redhat.vscode-yaml) extension.
2. Open VS Code's settings (`CTRL+,`)
3. Add a mapping for our schema.

```js
{
  "yaml.schemas": {
    "https://raw.githubusercontent.com/ezplatform/ezob/main/src/schema/ezob-schema.json": [
      "tye.yaml"
    ]
  }
}
```