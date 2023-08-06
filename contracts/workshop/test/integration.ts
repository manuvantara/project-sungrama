import { loadFixture } from "@nomicfoundation/hardhat-network-helpers";
import { expect } from "chai";
import { ethers } from "hardhat";

describe("Integration", function () {
  async function deployWorkshopAndCollectionFixture() {
    const [owner, user, ...otherAccounts] = await ethers.getSigners();

    const Collection = await ethers.getContractFactory("MyCollection");
    const collection = await Collection.deploy();

    const Workshop = await ethers.getContractFactory("MyWorkshop");
    const workshop = await Workshop.deploy(collection.address);

    return { workshop, collection, owner, user, otherAccounts };
  }

  const RECIPE = {
    inputIds: [1],
    inputAmounts: [50],
    outputIds: [2],
    outputAmounts: [1],
  };

  describe("Craft Function", function () {
    it("should revert if trying to craft with insufficient token balances", async function () {
      const { workshop, user } = await loadFixture(
        deployWorkshopAndCollectionFixture
      );
      const { inputIds, inputAmounts, outputIds, outputAmounts } = RECIPE;

      const recipeId = await workshop.hashRecipe(
        inputIds,
        inputAmounts,
        outputIds,
        outputAmounts
      );
      await workshop.createRecipe(
        inputIds,
        inputAmounts,
        outputIds,
        outputAmounts
      );

      await expect(
        workshop.connect(user).craft(recipeId)
      ).to.be.revertedWithCustomError(workshop, "InsufficientBalance");
    });

    it("should revert if trying to craft without workshop being allowed to manage user's tokens", async function () {
      const { collection, workshop, user } = await loadFixture(
        deployWorkshopAndCollectionFixture
      );
      const { inputIds, inputAmounts, outputIds, outputAmounts } = RECIPE;

      // prepare recipe
      const recipeId = await workshop.hashRecipe(
        inputIds,
        inputAmounts,
        outputIds,
        outputAmounts
      );
      await workshop.createRecipe(
        inputIds,
        inputAmounts,
        outputIds,
        outputAmounts
      );

      // prepare tokens
      await collection.mintBatch(user.address, inputIds, inputAmounts, "0x");

      // autorize Workshop as a minter
      const WORKSHOP_ROLE = await collection.WORKSHOP_ROLE();
      await collection.grantRole(WORKSHOP_ROLE, workshop.address);

      // craft
      await expect(workshop.connect(user).craft(recipeId)).to.be.reverted;

      // check balances
      for (let i = 0; i < inputIds.length; i++) {
        const amount = await collection.balanceOf(user.address, inputIds[i]);
        expect(amount == inputIds[i]);
      }

      for (let i = 0; i < outputIds.length; i++) {
        const amount = await collection.balanceOf(user.address, outputIds[i]);
        expect(amount == 0);
      }
    });

    it("should revert if trying to craft without workshop being autorized as a minter", async function () {
      const { collection, workshop, user } = await loadFixture(
        deployWorkshopAndCollectionFixture
      );
      const { inputIds, inputAmounts, outputIds, outputAmounts } = RECIPE;

      // prepare recipe
      const recipeId = await workshop.hashRecipe(
        inputIds,
        inputAmounts,
        outputIds,
        outputAmounts
      );
      await workshop.createRecipe(
        inputIds,
        inputAmounts,
        outputIds,
        outputAmounts
      );

      // prepare tokens
      await collection.mintBatch(user.address, inputIds, inputAmounts, "0x");

      // allow Workshop to manage user's tokens
      await collection.connect(user).setApprovalForAll(workshop.address, true);

      // craft
      await expect(workshop.connect(user).craft(recipeId)).to.be.reverted;

      // check balances
      for (let i = 0; i < inputIds.length; i++) {
        const amount = await collection.balanceOf(user.address, inputIds[i]);
        expect(amount == inputIds[i]);
      }

      for (let i = 0; i < outputIds.length; i++) {
        const amount = await collection.balanceOf(user.address, outputIds[i]);
        expect(amount == 0);
      }
    });

    it("should allow crafting with valid recipe, workshop autorization and update token balances correctly", async function () {
      const { collection, workshop, user } = await loadFixture(
        deployWorkshopAndCollectionFixture
      );
      const { inputIds, inputAmounts, outputIds, outputAmounts } = RECIPE;

      // prepare recipe
      const recipeId = await workshop.hashRecipe(
        inputIds,
        inputAmounts,
        outputIds,
        outputAmounts
      );
      await workshop.createRecipe(
        inputIds,
        inputAmounts,
        outputIds,
        outputAmounts
      );

      // prepare tokens
      await collection.mintBatch(user.address, inputIds, inputAmounts, "0x");

      // autorize Workshop as a minter
      const WORKSHOP_ROLE = await collection.WORKSHOP_ROLE();
      await collection.grantRole(WORKSHOP_ROLE, workshop.address);

      // allow Workshop to manage user's tokens
      await collection.connect(user).setApprovalForAll(workshop.address, true);

      // craft
      await expect(workshop.connect(user).craft(recipeId))
        .to.emit(workshop, "Crafted")
        .withArgs(recipeId, user.address, outputIds, outputAmounts);

      // check balances
      for (let i = 0; i < inputIds.length; i++) {
        const amount = await collection.balanceOf(user.address, inputIds[i]);
        expect(amount == 0);
      }

      for (let i = 0; i < outputIds.length; i++) {
        const amount = await collection.balanceOf(user.address, outputIds[i]);
        expect(amount == outputIds[i]);
      }
    });
  });
});
