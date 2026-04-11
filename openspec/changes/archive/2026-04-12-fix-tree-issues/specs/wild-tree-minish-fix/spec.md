## ADDED Requirements

### Requirement: Wild tree minish works when texture change is disabled
当野树启用 `MinishTree` 但 `TextureChange` 关闭时，野树 SHALL be rendered at half size.

#### Scenario: Wild tree shrinks when only MinishTree is enabled
- **WHEN** `MinishTree` is enabled AND `TextureChange` is disabled
- **THEN** wild trees SHALL be rendered at 50% scale
- **AND** the tree trunk and shadow SHALL be scaled proportionally

#### Scenario: Wild tree behavior unchanged when TextureChange is enabled
- **WHEN** `TextureChange` is enabled
- **THEN** wild trees SHALL behave as before (texture replacement takes precedence)

#### Scenario: Consistent behavior between wild trees and fruit trees
- **WHEN** `MinishTree` is enabled AND `TextureChange` is disabled
- **THEN** both wild trees and fruit trees SHALL be rendered at half size
