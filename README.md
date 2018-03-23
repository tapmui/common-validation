# collector-common-validation

A collection of string validation tools available as Nuget packages. Currently includes the following validation packages:

| Validation subject | Countries | Nuget package name |
| ------------------ | --------- | ------------------ |
| National Identifiers (Social Security Numbers) | Denmark, Finland, Norway, Sweden | Collector.Common.Validation.NationalIdentifier |

## Installing

Download the package of choice from Nuget:

```Powershell
PM> install-package Collector.Common.Validation.NationalIdentifier
```

## Usage

### National Identifiers

Validate a number for a specific country using any official format (returns a bool):

```c#
var nationalIdentifier = "200301046835";
var validator = new SwedishNationalIdentifierValidator();
var result = validator.IsValid(nationalIdentifier);

// true
```

or 

```c#
var nationalIdentifier = "200301046835";
var validator = NationalIdentifierValidator.GetValidator(CountryCode.SE);
var result = validator.IsValid(nationalIdentifier);

// true
```

Normalize national identifiers to a single format:

```c#
var nationalIdentifier = "20030104-6835";
var validator = new SwedishIdentifierValidator();
var result = validator.Normalize(nationalIdentifier);

// "200301046835"
```

Check for validity or normalize an identifier for any of the supported countries:

```c#
var result = NationalIdentifierValidator.IsValidInAnyCountry(nationalIdentifier);
var normalizedIdentifier = NationalIdentifierValidator.NormalizeForAnyCountry(nationalIdentifier);
```

Validate a property with data annotations:

```c#
[NationalIdentifier]
public string NationalIdentifier { get; set; }

[NationalIdentifier(CountryCode.SE)]
public string NationalIdentifier { get; set; }
```

## Contributing

Extensions are very welcome! Get in touch with the Solutions Team and open a pull request.

## Acknowledgments

* Thanks to Said Outgajjouft for the initial work on National Identifiers.
