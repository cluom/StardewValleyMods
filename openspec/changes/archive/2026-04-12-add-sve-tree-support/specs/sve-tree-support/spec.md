## ADDED Requirements

### Requirement: SVE fruit tree detection
The system SHALL detect SVE (Stardew Valley Expanded) fruit trees by checking if the tree's texture path contains `FlashShifter.StardewValleyExpandedCP`.

#### Scenario: Detect SVE pear tree
- **WHEN** a fruit tree has texture path containing `FlashShifter.StardewValleyExpandedCP` and TextureSpriteRow is 0
- **THEN** the system SHALL identify it as an SVE Pear tree

#### Scenario: Detect SVE nectarine tree
- **WHEN** a fruit tree has texture path containing `FlashShifter.StardewValleyExpandedCP` and TextureSpriteRow is 1
- **THEN** the system SHALL identify it as an SVE Nectarine tree

#### Scenario: Detect SVE persimmon tree
- **WHEN** a fruit tree has texture path containing `FlashShifter.StardewValleyExpandedCP` and TextureSpriteRow is 2
- **THEN** the system SHALL identify it as an SVE Persimmon tree

#### Scenario: Detect SVE money tree
- **WHEN** a fruit tree has texture path containing `FlashShifter.StardewValleyExpandedCP` and TextureSpriteRow is 3
- **THEN** the system SHALL identify it as an SVE Money Tree

### Requirement: SVE tree texture replacement
The system SHALL allow users to enable or disable texture replacement for each SVE fruit tree type independently via configuration toggles.

#### Scenario: Enable pear texture replacement
- **WHEN** `ChangePear` config is true
- **THEN** the system SHALL apply the configured replacement texture to SVE Pear trees

#### Scenario: Disable nectarine texture replacement
- **WHEN** `ChangeNectarine` config is false
- **THEN** the system SHALL NOT modify the texture of SVE Nectarine trees

#### Scenario: Enable persimmon texture replacement
- **WHEN** `ChangePersimmon` config is true
- **THEN** the system SHALL apply the configured replacement texture to SVE Persimmon trees

#### Scenario: Enable money tree texture replacement
- **WHEN** `ChangeMoneyTree` config is true
- **THEN** the system SHALL apply the configured replacement texture to SVE Money Trees

### Requirement: SVE tree configuration menu
The system SHALL display SVE fruit tree options in a dedicated `sve_fruit_trees` configuration page when SVE mod is loaded, mirroring the RSV `ridgeside_fruit_trees` page pattern.

#### Scenario: Show SVE config page when SVE is loaded
- **WHEN** SVE mod (`FlashShifter.StardewValleyExpandedCP`) is installed and loaded
- **THEN** the configuration menu SHALL show a page link labeled "SVE Fruit Trees"
- **AND** the page SHALL contain toggles for Pear, Nectarine, Persimmon, and Money Tree

#### Scenario: Hide SVE config page when SVE is not loaded
- **WHEN** SVE mod is not installed
- **THEN** the configuration menu SHALL NOT show SVE fruit tree options

### Requirement: SVE tree type registration
The system SHALL register SVE tree types in the tree type enum when SVE mod is loaded.

#### Scenario: Register SVE tree types conditionally
- **WHEN** SVE mod is loaded and `Config.ChangePear` is true
- **THEN** the system SHALL register the Pear tree type for texture replacement

### Requirement: SVE wild tree detection
The system SHALL detect SVE wild trees (Birch, Fir) by checking if the treeType value starts with `FlashShifter.StardewValleyExpandedCP`.

#### Scenario: Detect SVE birch tree
- **WHEN** a tree has treeType starting with `FlashShifter.StardewValleyExpandedCP` and is identified as Birch
- **THEN** the system SHALL apply wild tree texture replacement logic

#### Scenario: Detect SVE fir tree
- **WHEN** a tree has treeType starting with `FlashShifter.StardewValleyExpandedCP` and is identified as Fir
- **THEN** the system SHALL apply wild tree texture replacement logic

### Requirement: SVE wild tree texture replacement
The system SHALL allow texture replacement for SVE wild trees (Birch, Fir) via the existing wild tree mechanism in TreePatch.

#### Scenario: Enable SVE birch texture replacement
- **WHEN** SVE is loaded and `ChangeTreeType` is called with SVE Birch treeType
- **THEN** the system SHALL add the treeType to `ControlTreeTypeValues` for texture replacement

#### Scenario: Enable SVE fir texture replacement
- **WHEN** SVE is loaded and `ChangeTreeType` is called with SVE Fir treeType
- **THEN** the system SHALL add the treeType to `ControlTreeTypeValues` for texture replacement
