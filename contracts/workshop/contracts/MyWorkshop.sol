// SPDX-License-Identifier: MIT
pragma solidity ^0.8.9;

import "./Workshop.sol";
import "./Collection.sol";

contract MyWorkshop is Workshop {
    constructor(Collection collectionAddress) Workshop(collectionAddress) {}
}
