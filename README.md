# [[EC] Armenian Encoding Converter](https://github.com/mrzaxaryan/EC)

[![Build Status](https://github.com/mrzaxaryan/EC/actions/workflows/dotnet.yml/badge.svg)](https://github.com/mrzaxaryan/EC/actions)

## Overview

The **Armenian Encoding Converter (EC)** is a .NET 9 application designed to facilitate the conversion of Armenian text between various encoding formats. It provides a library for encoding conversions, a command-line interface (CLI) for user interaction, and unit tests to ensure reliability.

## Features

- **Library (`EC.Library`)**:
  - `TextConverter`: Convert text files between different Armenian encodings.
  - `ExcelConverter`: Handle encoding conversions in Excel files.
  - `WordConverter`: Process Word documents for encoding transformations.
  - `EncodingMapper`: Core logic for mapping between encoding types.
  - `EncodingType`: Enumeration of supported encoding types.

- **Command-Line Interface (`EC.CLI`)**:
  - A simple and intuitive CLI for performing encoding conversions.

- **Unit Tests (`EC.Library.Tests`)**:
  - Comprehensive test coverage for core functionality and converters.

## Getting Started

### Prerequisites

- .NET 9 SDK installed on your system.

### Installation

1. Clone the repository:
```
   git clone https://github.com/mrzaxaryan/EC.git
   cd EC
```

2. Build the solution:
```
   dotnet build
```

3. Run the tests:
```
   dotnet test
```

### Usage

To use the CLI for encoding conversion:
```
dotnet run --project EC.CLI
```

Follow the on-screen instructions to perform encoding conversions.

## Project Structure

- **`EC.Library`**: Core library for encoding conversions.
- **`EC.CLI`**: Command-line interface for user interaction.
- **`EC.Library.Tests`**: Unit tests for the library.

## Contributing

Contributions are welcome! Please fork the repository and submit a pull request.

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.

## Acknowledgments

Special thanks to the contributors and the open-source community for their support.