## ADDED Requirements

### Requirement: Configurable seed tip visibility for trees with tappers
当树装备了树液采集器时，系统 SHALL 根据配置 `ShowTreeSeedTipWhenTapperInstalled` 决定是否显示种子提示。

#### Scenario: Hide seed tip by default when tapper installed
- **WHEN** a tree has a tapper attached AND `ShowTreeSeedTipWhenTapperInstalled` is false (default)
- **THEN** the seed tip SHALL NOT be displayed for that tree

#### Scenario: Show seed tip when configured to do so
- **WHEN** a tree has a tapper attached AND `ShowTreeSeedTipWhenTapperInstalled` is true
- **THEN** the seed tip SHALL be displayed when the tree has seeds (even though player cannot shake the tree)

#### Scenario: Show seed tip for tree without tapper
- **WHEN** a tree does not have a tapper attached AND `ShowTreeSeedTips` is enabled
- **THEN** the seed tip SHALL be displayed when the tree has seeds

#### Scenario: Taper detection is accurate
- **WHEN** checking if a tree has a tapper
- **THEN** the system SHALL check the tile where the tree is located for any object with `IsTapper()` returning true
