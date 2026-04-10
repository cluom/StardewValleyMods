## ADDED Requirements

### Requirement: Ridgeside Village tree type recognition
The system SHALL recognize and process Ridgeside Village's 8 custom fruit tree types when installed. Tree types include Cherry Pluot, Desert Tangelo, Ember Blood Lime, Highland Jostaberry, Mountain Plumcot, Northern Limequat, Paradise Rangpur, and Tropi Ugli Fruit.

#### Scenario: Tree type identifier matching
- **WHEN** a tree's `treeType` value starts with `Rafseazz.RSVCP`
- **THEN** the system SHALL match it against known Ridgeside Village tree type names

#### Scenario: Unrecognized Ridgeside tree types
- **WHEN** a tree's `treeType` starts with `Rafseazz.RSVCP` but does not match known Ridgeside Village tree names
- **THEN** the system SHALL skip processing for that tree without error

### Requirement: Per-tree-type configuration toggles
The system SHALL provide boolean configuration options for each Ridgeside Village tree type, allowing users to enable or disable ControlTree features for specific trees.

#### Scenario: Config toggle defaults
- **WHEN** a user first loads the mod with Ridgeside Village installed
- **THEN** all Ridgeside Village tree type toggles SHALL default to `true` (enabled)

#### Scenario: Individual tree type disabled
- **WHEN** a user sets `ChangeCherryPluot` to `false`
- **THEN** no ControlTree highlighting, texture replacement, or minimization SHALL be applied to Cherry Pluot trees

### Requirement: Integration with existing tree processing
Ridgeside Village trees SHALL be fully integrated with existing ControlTree features including seed/sapling highlighting, texture replacement, tree minimization, and transparency effects.

#### Scenario: Seed highlighting on Ridgeside saplings
- **WHEN** a Ridgeside Village sapling is in seed or sapling growth stage
- **AND** `HighlightTreeSeed` or `HighlightSapling` is enabled
- **AND** the tree's type toggle is `true`
- **THEN** the highlight box SHALL be drawn around the sapling

#### Scenario: Texture replacement on mature Ridgeside trees
- **WHEN** a mature Ridgeside Village tree exists
- **AND** `TextureChange` is enabled
- **AND** the tree's type toggle is `true`
- **THEN** texture replacement logic SHALL attempt to apply to the tree

#### Scenario: Minimization on Ridgeside trees
- **WHEN** a Ridgeside Village tree exists
- **AND** `MinishTree` is enabled
- **AND** the tree's type toggle is `true`
- **THEN** the tree SHALL be rendered at 50% scale
