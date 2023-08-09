import { loadFixture } from "@nomicfoundation/hardhat-network-helpers";
import { expect } from "chai";
import { ethers } from "hardhat";

describe("Integration", function () {
  async function deployWorkbenchAndCollectionFixture() {
    const [owner, user, ...otherAccounts] = await ethers.getSigners();

    const Collection = await ethers.getContractFactory("MyCollection");
    const collection = await Collection.deploy();

    const Workbench = await ethers.getContractFactory("MyWorkbench");
    const workbench = await Workbench.deploy(collection.address);

    return { workbench, collection, owner, user, otherAccounts };
  }

  const BLUEPRINT = {
    inputIds: [1],
    inputAmounts: [50],
    outputIds: [2],
    outputAmounts: [1],
  };

  describe("Craft Function", function () {
    it("should revert if trying to craft with insufficient token balances", async function () {
      const { workbench, user } = await loadFixture(
        deployWorkbenchAndCollectionFixture
      );
      const { inputIds, inputAmounts, outputIds, outputAmounts } = BLUEPRINT;

      const blueprintId = await workbench.hashBlueprint(
        inputIds,
        inputAmounts,
        outputIds,
        outputAmounts
      );
      await workbench.createBlueprint(
        inputIds,
        inputAmounts,
        outputIds,
        outputAmounts
      );

      await expect(
        workbench.connect(user).craft(blueprintId)
      ).to.be.revertedWithCustomError(workbench, "InsufficientBalance");
    });

    it("should revert if trying to craft without workbench being allowed to manage user's tokens", async function () {
      const { collection, workbench, user } = await loadFixture(
        deployWorkbenchAndCollectionFixture
      );
      const { inputIds, inputAmounts, outputIds, outputAmounts } = BLUEPRINT;

      // prepare blueprint
      const blueprintId = await workbench.hashBlueprint(
        inputIds,
        inputAmounts,
        outputIds,
        outputAmounts
      );
      await workbench.createBlueprint(
        inputIds,
        inputAmounts,
        outputIds,
        outputAmounts
      );

      // prepare tokens
      await collection.mintBatch(user.address, inputIds, inputAmounts, "0x");

      // autorize Workbench as a minter
      const WORKBENCH_ROLE = await collection.WORKBENCH_ROLE();
      await collection.grantRole(WORKBENCH_ROLE, workbench.address);

      // craft
      await expect(workbench.connect(user).craft(blueprintId)).to.be.reverted;

      // check balances
      for (let i = 0; i < inputIds.length; i++) {
        const amount = await collection.balanceOf(user.address, inputIds[i]);
        expect(amount).to.equal(inputAmounts[i]);
      }

      for (let i = 0; i < outputIds.length; i++) {
        const amount = await collection.balanceOf(user.address, outputIds[i]);
        expect(amount).to.equal(0);
      }
    });

    it("should revert if trying to craft without workbench being autorized as a minter", async function () {
      const { collection, workbench, user } = await loadFixture(
        deployWorkbenchAndCollectionFixture
      );
      const { inputIds, inputAmounts, outputIds, outputAmounts } = BLUEPRINT;

      // prepare blueprint
      const blueprintId = await workbench.hashBlueprint(
        inputIds,
        inputAmounts,
        outputIds,
        outputAmounts
      );
      await workbench.createBlueprint(
        inputIds,
        inputAmounts,
        outputIds,
        outputAmounts
      );

      // prepare tokens
      await collection.mintBatch(user.address, inputIds, inputAmounts, "0x");

      // allow Workbench to manage user's tokens
      await collection.connect(user).setApprovalForAll(workbench.address, true);

      // craft
      await expect(workbench.connect(user).craft(blueprintId)).to.be.reverted;

      // check balances
      for (let i = 0; i < inputIds.length; i++) {
        const amount = await collection.balanceOf(user.address, inputIds[i]);
        expect(amount).to.equal(inputAmounts[i]);
      }

      for (let i = 0; i < outputIds.length; i++) {
        const amount = await collection.balanceOf(user.address, outputIds[i]);
        expect(amount).to.equal(0);
      }
    });

    it("should allow crafting with valid blueprint, workbench autorization and update token balances correctly", async function () {
      const { collection, workbench, user } = await loadFixture(
        deployWorkbenchAndCollectionFixture
      );
      const { inputIds, inputAmounts, outputIds, outputAmounts } = BLUEPRINT;

      // prepare blueprint
      const blueprintId = await workbench.hashBlueprint(
        inputIds,
        inputAmounts,
        outputIds,
        outputAmounts
      );
      await workbench.createBlueprint(
        inputIds,
        inputAmounts,
        outputIds,
        outputAmounts
      );

      // prepare tokens
      await collection.mintBatch(user.address, inputIds, inputAmounts, "0x");

      // autorize Workbench as a minter
      const WORKBENCH_ROLE = await collection.WORKBENCH_ROLE();
      await collection.grantRole(WORKBENCH_ROLE, workbench.address);

      // allow Workbench to manage user's tokens
      await collection.connect(user).setApprovalForAll(workbench.address, true);

      // craft
      await expect(workbench.connect(user).craft(blueprintId))
        .to.emit(workbench, "Crafted")
        .withArgs(blueprintId, user.address, outputIds, outputAmounts);

      // check balances
      for (let i = 0; i < inputIds.length; i++) {
        const amount = await collection.balanceOf(user.address, inputIds[i]);
        expect(amount).to.equal(0);
      }

      for (let i = 0; i < outputIds.length; i++) {
        const amount = await collection.balanceOf(user.address, outputIds[i]);
        expect(amount).to.equal(outputAmounts[i]);
      }
    });
  });
});
