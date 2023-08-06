// SPDX-License-Identifier: MIT
pragma solidity ^0.8.9;

/**
 * @dev Interface of the Workshop.
 */
interface IWorkshop {
    /**
     * @dev Crafting recipe structure.
     */
    struct Recipe {
        uint256[] inputIds;
        uint256[] inputAmounts;
        uint256[] outputIds;
        uint256[] outputAmounts;
    }

    /**
     * @dev Empty array or a mismatch between the ids and amounts length for a create recipe call.
     */
    error InvalidRecipeLength(
        uint256 inputIdsLength,
        uint256 inputAmountsLength,
        uint256 outputIdsLength,
        uint256 outputAmountsLength
    );

    /**
     * @dev The `recipeId` already exist.
     */
    error RecipeAlreadyExists(uint256 recipeId);

    /**
     * @dev The `recipeId` does not exist.
     */
    error RecipeNotFound(uint256 recipeId);

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
     * @dev Emitted when a recipe is created.
     */
    event RecipeCreated(uint256 recipeId);

    /**
     * @dev Emitted when the recipe is deleted.
     */
    event RecipeDeleted(uint256 recipeId);

    /**
     * @dev Emitted when a craft action is executed
     */
    event Crafted(
        uint256 recipeId,
        address account,
        uint256[] outputIds,
        uint256[] outputAmounts
    );

    /**
     * @dev Hashing function used to (re)build the recipe id from its details.
     */
    function hashRecipe(
        uint256[] calldata inputIds,
        uint256[] calldata inputAmounts,
        uint256[] calldata outputIds,
        uint256[] calldata outputAmounts
    ) external pure returns (uint256 recipeId);

    /**
     * @dev Function to create a new crafting recipe.
     */
    function createRecipe(
        uint256[] calldata inputIds,
        uint256[] calldata inputAmounts,
        uint256[] calldata outputIds,
        uint256[] calldata outputAmounts
    ) external returns (uint256 newRecipeId);

    /**
     * @dev Function to delete a crafting recipe.
     */
    function deleteRecipe(uint256 recipeId) external;

    /**
     * @dev Craft token(s) using a given crafting recipe.
     * Burns input tokens and mints output tokens.
     */
    function craft(uint256 recipeId) external;
}
