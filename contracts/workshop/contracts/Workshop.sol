// SPDX-License-Identifier: MIT
pragma solidity ^0.8.9;

import "@openzeppelin/contracts/access/Ownable.sol";
import "./IWorkshop.sol";
import "./Collection.sol";

abstract contract Workshop is Ownable, IWorkshop {
    /**
     * @dev Token collection. The collection should inherit from ERC1155Burnable
     * and register Workshop as a minter
     */
    Collection public immutable collection;
    mapping(uint256 => Recipe) private _recipes;

    /**
     * @dev Sets the token collection.
     */
    constructor(Collection collectionAddress) {
        collection = Collection(address(collectionAddress));
    }

    /**
     * @dev See IWorkshop
     */
    function hashRecipe(
        uint256[] calldata inputIds,
        uint256[] calldata inputAmounts,
        uint256[] calldata outputIds,
        uint256[] calldata outputAmounts
    ) public pure virtual override returns (uint256 recipeId) {
        return
            uint256(
                keccak256(
                    abi.encode(inputIds, inputAmounts, outputIds, outputAmounts)
                )
            );
    }

    /**
     * @dev Validates that amounts are not zero.
     */
    function _validateAmounts(
        uint256[] calldata inputAmounts,
        uint256[] calldata outputAmounts
    ) private pure {
        for (uint256 i = 0; i < inputAmounts.length; i++) {
            if (inputAmounts[i] == 0) {
                revert InvalidAmount(inputAmounts[i]);
            }
        }

        for (uint256 i = 0; i < outputAmounts.length; i++) {
            if (outputAmounts[i] == 0) {
                revert InvalidAmount(outputAmounts[i]);
            }
        }
    }

    /**
     * @dev See IWorkshop
     */
    function createRecipe(
        uint256[] calldata inputIds,
        uint256[] calldata inputAmounts,
        uint256[] calldata outputIds,
        uint256[] calldata outputAmounts
    ) external virtual override onlyOwner returns (uint256 newRecipeId) {
        if (
            inputIds.length != inputAmounts.length ||
            outputIds.length != outputAmounts.length ||
            inputIds.length == 0 ||
            outputIds.length == 0
        )
            revert InvalidRecipeLength(
                inputIds.length,
                inputAmounts.length,
                outputIds.length,
                outputAmounts.length
            );
        _validateAmounts(inputAmounts, outputAmounts);

        newRecipeId = hashRecipe(
            inputIds,
            inputAmounts,
            outputIds,
            outputAmounts
        );
        if (_recipes[newRecipeId].outputIds.length != 0) {
            revert RecipeAlreadyExists(newRecipeId);
        }
        _recipes[newRecipeId] = Recipe({
            inputIds: inputIds,
            inputAmounts: inputAmounts,
            outputIds: outputIds,
            outputAmounts: outputAmounts
        });
        emit RecipeCreated(newRecipeId);

        return newRecipeId;
    }

    /**
     * @dev Modifier to check if a recipe with the given ID exists
     * before function execution.
     */
    modifier recipeExists(uint256 recipeId) {
        if (_recipes[recipeId].outputIds.length == 0) {
            revert RecipeNotFound(recipeId);
        }
        _;
    }

    /**
     * @dev See IWorkshop
     */
    function deleteRecipe(
        uint256 recipeId
    ) external virtual override onlyOwner recipeExists(recipeId) {
        delete _recipes[recipeId];
        emit RecipeDeleted(recipeId);
    }

    /**
     * @dev Checks token balances of a given account against
     * the required amounts to execute craft.
     */
    function _checkTokenBalances(
        address account,
        uint256[] memory inputIds,
        uint256[] memory inputAmounts
    ) private view {
        for (uint256 i = 0; i < inputIds.length; i++) {
            uint256 amount = collection.balanceOf(account, inputIds[i]);
            if (amount < inputAmounts[i]) revert InsufficientBalance();
        }
    }

    /**
     * @dev See IWorkshop
     */
    function craft(uint256 recipeId) external recipeExists(recipeId) {
        address account = msg.sender;
        Recipe storage recipe = _recipes[recipeId];

        _checkTokenBalances(account, recipe.inputIds, recipe.inputAmounts);

        collection.burnBatch(account, recipe.inputIds, recipe.inputAmounts);
        collection.mintBatch(
            account,
            recipe.outputIds,
            recipe.outputAmounts,
            "0x"
        );
        emit Crafted(recipeId, account, recipe.outputIds, recipe.outputAmounts);
    }
}
