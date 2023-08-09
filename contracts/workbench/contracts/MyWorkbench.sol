// SPDX-License-Identifier: MIT
pragma solidity ^0.8.9;

import "./Workbench.sol";
import "./Collection.sol";

contract MyWorkbench is Workbench {
    constructor(Collection collectionAddress) Workbench(collectionAddress) {}
}
