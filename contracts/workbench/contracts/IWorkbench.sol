// SPDX-License-Identifier: MIT
pragma solidity ^0.8.9;

/**
 * @dev Interface of the Workbench.
 */
interface IWorkbench {
    /**
     * @dev Crafting blueprint structure.
     */
    struct Blueprint {
        uint256[] inputIds;
        uint256[] inputAmounts;
        uint256[] outputIds;
        uint256[] outputAmounts;
    }

    /**
     * @dev Empty array or a mismatch between the ids and amounts length for a create blueprint call.
     */
    error InvalidBlueprintLength(
        uint256 inputIdsLength,
        uint256 inputAmountsLength,
        uint256 outputIdsLength,
        uint256 outputAmountsLength
    );

    /**
     * @dev The `blueprintId` already exist.
     */
    error BlueprintAlreadyExists(uint256 blueprintId);

    /**
     * @dev The `blueprintId` does not exist.
     */
    error BlueprintNotFound(uint256 blueprintId);

    /**
     * @dev The amount was set to invalid value i.e. zero.
     */
    error InvalidAmount(uint256 amount);

    /**
     * @dev The amount was set to invalid value i.e. zero.
     */
    error DuplicateId(uint256 id);

    /**
     * @dev There is not enough tokens to initiate craft.
     */
    error InsufficientBalance();

    /**
     * @dev Emitted when a blueprint is created.
     */
    event BlueprintCreated(uint256 blueprintId);

    /**
     * @dev Emitted when the blueprint is deleted.
     */
    event BlueprintDeleted(uint256 blueprintId);

    /**
     * @dev Emitted when a craft action is executed
     */
    event Crafted(
        uint256 blueprintId,
        address account,
        uint256[] outputIds,
        uint256[] outputAmounts
    );

    /**
     * @dev Hashing function used to (re)build the blueprint id from its details.
     */
    function hashBlueprint(
        uint256[] calldata inputIds,
        uint256[] calldata inputAmounts,
        uint256[] calldata outputIds,
        uint256[] calldata outputAmounts
    ) external pure returns (uint256 blueprintId);

    /**
     * @dev Function to create a new blueprint.
     */
    function createBlueprint(
        uint256[] calldata inputIds,
        uint256[] calldata inputAmounts,
        uint256[] calldata outputIds,
        uint256[] calldata outputAmounts
    ) external returns (uint256 newBlueprintId);

    /**
     * @dev Function to delete a blueprint.
     */
    function deleteBlueprint(uint256 blueprintId) external;

    /**
     * @dev Craft token(s) using a given blueprint.
     * Burns input tokens and mints output tokens.
     */
    function craft(uint256 blueprintId) external;
}
