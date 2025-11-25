# [[EC] Armenian Encoding Converter](https://github.com/mrzaxaryan/EC)

[![Build Status](https://github.com/mrzaxaryan/EC/actions/workflows/dotnet.yml/badge.svg)](https://github.com/mrzaxaryan/EC/actions)

## Overview

The **Armenian Encoding Converter (EC)** is a .NET 9 application for converting Armenian text between encoding formats. It provides a core library, a command-line interface (CLI), a GUI (in progress), and unit tests.

## Table of Contents
- [Features](#features)
- [Getting Started](#getting-started)
- [Mapping File](#mapping-file)
- [Usage](#usage)
- [Project Structure](#project-structure)
- [Troubleshooting](#troubleshooting)
- [Contributing](#contributing)
- [License](#license)
- [Acknowledgments](#acknowledgments)
- [References](#references)

## Features

- **Library (`EC.Library`)**:
  - `TextConverter`: Converts text using a mapping file.
  - `ExcelConverter`: Converts Excel files (requires Windows + Office).
  - `WordConverter`: Converts Word documents (requires Windows + Office).
  - `EncodingMapper`: Loads and applies character mappings from a file.
  - `EncodingType`: Enum for supported conversion directions.

- **Command-Line Interface (`EC.CLI`)**:
  - CLI for batch or single-file conversion of Word/Excel files.

- **Graphical User Interface (`EC.GUI`)** *(in progress)*:
  - Planned cross-platform GUI for encoding conversion.
  - Will provide an intuitive interface for file selection and conversion options.

- **Unit Tests (`EC.Library.Tests`)**:
  - Tests for core logic and converters.

## Getting Started

### Prerequisites

- .NET 9 SDK
- Windows OS (for Word/Excel conversion)
- Microsoft Office installed (for Word/Excel conversion)

### Installation

1. Clone the repository:
   ```sh
   git clone https://github.com/mrzaxaryan/EC.git
   cd EC
   ```
2. Build the solution:
   ```sh
   dotnet build
   ```
3. Run the tests:
   ```sh
   dotnet test
   ```

### Mapping File

The character mapping file `armenian.map` must be present at:
```
EC/Encodings/armenian.map
```
Each line should be in the format:
```
A=?
B=?
# Lines starting with # are comments
```

## Usage

To use the CLI for encoding conversion:
```sh
dotnet run --project EC.CLI -- --file <path-to-file> --type <0|1>
```
Or for batch conversion:
```sh
dotnet run --project EC.CLI -- --directory <path-to-directory> --type <0|1>
```
- `--type 0`: ANSI to Unicode
- `--type 1`: Unicode to ANSI

Supported file types:
- Word: `.doc`, `.docx`
- Excel: `.xls`, `.xlsx`

## Project Structure

- `EC.Library`: Core library
- `EC.CLI`: Command-line interface
- `EC.GUI`: Graphical user interface *(in progress)*
- `EC.Library.Tests`: Unit tests

## Troubleshooting

- **Office not installed**: Word/Excel conversion requires Microsoft Office.
- **Mapping file missing**: Ensure `armenian.map` exists at the specified path.
- **Unsupported file type**: Only Word/Excel files are supported for Office conversion.

## Contributing

Contributions are welcome! Please fork the repository and submit a pull request.

## License

MIT License. See [LICENSE](LICENSE).

## Acknowledgments

Special thanks to contributors and the open-source community.

## References

- [.NET 9 Documentation](https://docs.microsoft.com/en-us/dotnet/core/introduction)