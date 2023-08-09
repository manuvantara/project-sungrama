// SPDX-License-Identifier: MIT
pragma solidity ^0.8.9;

import "@openzeppelin/contracts/access/Ownable.sol";
import "./IWorkbench.sol";
import "./Collection.sol";

abstract contract Workbench is Ownable, IWorkbench {
    /**
     * @dev Token collection. The collection should inherit from ERC1155Burnable
     * and register Workbench as a minter
     */
    Collection public immutable collection;
    mapping(uint256 => Blueprint) private _blueprints;

    /**
     * @dev Sets the token collection.
     */
    constructor(Collection collectionAddress) {
        collection = Collection(address(collectionAddress));
    }

    /**
     * @dev See IWorkbench
     */
    function hashBlueprint(
        uint256[] calldata inputIds,
        uint256[] calldata inputAmounts,
        uint256[] calldata outputIds,
        uint256[] calldata outputAmounts
    ) public pure virtual override returns (uint256 blueprintId) {
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
     * @dev See IWorkbench
     */
    function createBlueprint(
        uint256[] calldata inputIds,
        uint256[] calldata inputAmounts,
        uint256[] calldata outputIds,
        uint256[] calldata outputAmounts
    ) external virtual override onlyOwner returns (uint256 newBlueprintId) {
        if (
            inputIds.length != inputAmounts.length ||
            outputIds.length != outputAmounts.length ||
            inputIds.length == 0 ||
            outputIds.length == 0
        )
            revert InvalidBlueprintLength(
                inputIds.length,
                inputAmounts.length,
                outputIds.length,
                outputAmounts.length
            );
        _validateAmounts(inputAmounts, outputAmounts);

        newBlueprintId = hashBlueprint(
            inputIds,
            inputAmounts,
            outputIds,
            outputAmounts
        );
        if (_blueprints[newBlueprintId].outputIds.length != 0) {
            revert BlueprintAlreadyExists(newBlueprintId);
        }
        _blueprints[newBlueprintId] = Blueprint({
            inputIds: inputIds,
            inputAmounts: inputAmounts,
            outputIds: outputIds,
            outputAmounts: outputAmounts
        });
        emit BlueprintCreated(newBlueprintId);
    }

    /**
     * @dev Modifier to check if a blueprint with the given ID exists
     * before function execution.
     */
    modifier blueprintExists(uint256 blueprintId) {
        if (_blueprints[blueprintId].outputIds.length == 0) {
            revert BlueprintNotFound(blueprintId);
        }
        _;
    }

    /**
     * @dev See IWorkbench
     */
    function deleteBlueprint(
        uint256 blueprintId
    ) external virtual override onlyOwner blueprintExists(blueprintId) {
        delete _blueprints[blueprintId];
        emit BlueprintDeleted(blueprintId);
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
     * @dev See IWorkbench
     */
    function craft(uint256 blueprintId) external blueprintExists(blueprintId) {
        address account = msg.sender;
        Blueprint storage blueprint = _blueprints[blueprintId];

        _checkTokenBalances(
            account,
            blueprint.inputIds,
            blueprint.inputAmounts
        );

        collection.burnBatch(
            account,
            blueprint.inputIds,
            blueprint.inputAmounts
        );
        collection.mintBatch(
            account,
            blueprint.outputIds,
            blueprint.outputAmounts,
            "0x"
        );
        emit Crafted(
            blueprintId,
            account,
            blueprint.outputIds,
            blueprint.outputAmounts
        );
    }
}
