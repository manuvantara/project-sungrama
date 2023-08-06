import { loadFixture } from "@nomicfoundation/hardhat-network-helpers";
import { expect } from "chai";
import { ethers } from "hardhat";

describe("Workshop", function () {
  const COLLECTION_ADDRESS = "0xd9145CCE52D386f254917e481eB44e9943F39138";

  async function deployWorkshopFixture() {
    const [owner, ...otherAccounts] = await ethers.getSigners();

    const Workshop = await ethers.getContractFactory("MyWorkshop");
    const workshop = await Workshop.deploy(COLLECTION_ADDRESS);

    return { workshop, owner, otherAccounts };
  }

  describe("Deployment", function () {
    it("should set a collection address", async function () {
      const { workshop } = await loadFixture(deployWorkshopFixture);

      expect(workshop.collection).not.to.equal(COLLECTION_ADDRESS);
    });
  });

  const NON_EXISTING_RECIPE_ID = 12345n;
  const TEST_RECIPES = {
    empty: [
      {
        inputIds: [],
        inputAmounts: [],
        outputIds: [],
        outputAmounts: [],
      },
      {
        inputIds: [],
        inputAmounts: [50],
        outputIds: [2],
        outputAmounts: [1],
      },
      {
        inputIds: [1],
        inputAmounts: [],
        outputIds: [2],
        outputAmounts: [1],
      },
      {
        inputIds: [1],
        inputAmounts: [50],
        outputIds: [],
        outputAmounts: [1],
      },
      {
        inputIds: [1],
        inputAmounts: [50],
        outputIds: [2],
        outputAmounts: [],
      },
    ],
    mismatch: [
      {
        inputIds: [1, 2],
        inputAmounts: [10, 20, 30],
        outputIds: [4, 5],
        outputAmounts: [40, 50],
      },
      {
        inputIds: [1, 2],
        inputAmounts: [10, 20, 30],
        outputIds: [4, 5],
        outputAmounts: [40],
      },
    ],
    valid: {
      inputIds: [1, 2, 3],
      inputAmounts: [10, 20, 30],
      outputIds: [4, 5],
      outputAmounts: [40, 50],
    },
    zeroAmount: [
      {
        inputIds: [1, 2, 3],
        inputAmounts: [10, 0, 30],
        outputIds: [4, 5, 6],
        outputAmounts: [40, 50, 60],
      },
      {
        inputIds: [1, 2, 3],
        inputAmounts: [10, 20, 30],
        outputIds: [4, 5, 6],
        outputAmounts: [40, 0, 60],
      },
    ],
  };

  describe("Hash Crafting Recipe Function", function () {
    it("should hash the recipe correctly", async function () {
      const { workshop } = await loadFixture(deployWorkshopFixture);
      const { inputIds, inputAmounts, outputIds, outputAmounts } =
        TEST_RECIPES.valid;

      const expectedRecipeId = ethers.BigNumber.from(
        ethers.utils.keccak256(
          ethers.utils.defaultAbiCoder.encode(
            ["uint256[]", "uint256[]", "uint256[]", "uint256[]"],
            [inputIds, inputAmounts, outputIds, outputAmounts]
          )
        )
      );
      const actualRecipeId = await workshop.hashRecipe(
        inputIds,
        inputAmounts,
        outputIds,
        outputAmounts
      );

      expect(actualRecipeId).to.equal(expectedRecipeId);
    });
  });

  describe("Create Crafting Recipe Function", function () {
    it("should revert in case of empty array as a parameter", async function () {
      const { workshop } = await loadFixture(deployWorkshopFixture);

      for (const scenario of TEST_RECIPES.empty) {
        const { inputIds, inputAmounts, outputIds, outputAmounts } = scenario;
        await expect(
          workshop.createRecipe(
            inputIds,
            inputAmounts,
            outputIds,
            outputAmounts
          )
        )
          .to.be.revertedWithCustomError(workshop, "InvalidRecipeLength")
          .withArgs(
            inputIds.length,
            inputAmounts.length,
            outputIds.length,
            outputAmounts.length
          );
      }
    });

    it("should revert if there is a mismatch between the ids and amounts length", async function () {
      const { workshop } = await loadFixture(deployWorkshopFixture);

      for (const scenario of TEST_RECIPES.mismatch) {
        const { inputIds, inputAmounts, outputIds, outputAmounts } = scenario;
        await expect(
          workshop.createRecipe(
            inputIds,
            inputAmounts,
            outputIds,
            outputAmounts
          )
        )
          .to.be.revertedWithCustomError(workshop, "InvalidRecipeLength")
          .withArgs(
            inputIds.length,
            inputAmounts.length,
            outputIds.length,
            outputAmounts.length
          );
      }
    });

    it("should revert if input or output amount is zero", async function () {
      const { workshop } = await loadFixture(deployWorkshopFixture);

      for (const scenario of TEST_RECIPES.zeroAmount) {
        const { inputIds, inputAmounts, outputIds, outputAmounts } = scenario;
        await expect(
          workshop.createRecipe(
            inputIds,
            inputAmounts,
            outputIds,
            outputAmounts
          )
        )
          .to.be.revertedWithCustomError(workshop, "InvalidAmount")
          .withArgs(0);
      }
    });

    it("should create a new recipe with valid parameters", async function () {
      const { workshop } = await loadFixture(deployWorkshopFixture);
      const { inputIds, inputAmounts, outputIds, outputAmounts } =
        TEST_RECIPES.valid;

      const expectedRecipeId = await workshop.hashRecipe(
        inputIds,
        inputAmounts,
        outputIds,
        outputAmounts
      );

      await expect(
        workshop.createRecipe(inputIds, inputAmounts, outputIds, outputAmounts)
      )
        .to.emit(workshop, "RecipeCreated")
        .withArgs(expectedRecipeId);
    });
  });

  describe("Delete Crafting Recipe Function", function () {
    it("should revert if trying to delete a non-existing recipe", async function () {
      const { workshop } = await loadFixture(deployWorkshopFixture);

      await expect(workshop.deleteRecipe(NON_EXISTING_RECIPE_ID))
        .to.be.revertedWithCustomError(workshop, "RecipeNotFound")
        .withArgs(NON_EXISTING_RECIPE_ID);
    });

    it("should revert if non-owner tries to delete a recipe", async function () {
      const { workshop, otherAccounts } = await loadFixture(
        deployWorkshopFixture
      );
      const { inputIds, inputAmounts, outputIds, outputAmounts } =
        TEST_RECIPES.valid;

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

      await expect(workshop.connect(otherAccounts[0]).deleteRecipe(recipeId)).to
        .be.reverted;
    });

    it("should allow owner to delete the existing recipe", async function () {
      const { workshop } = await loadFixture(deployWorkshopFixture);
      const { inputIds, inputAmounts, outputIds, outputAmounts } =
        TEST_RECIPES.valid;

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

      await expect(workshop.deleteRecipe(recipeId))
        .to.emit(workshop, "RecipeDeleted")
        .withArgs(recipeId);
    });
  });

  describe("Craft Function", function () {
    it("should revert if trying to craft with a non-existing recipe", async function () {
      const { workshop, otherAccounts } = await loadFixture(
        deployWorkshopFixture
      );

      await expect(
        workshop.connect(otherAccounts[0]).craft(NON_EXISTING_RECIPE_ID)
      )
        .to.be.revertedWithCustomError(workshop, "RecipeNotFound")
        .withArgs(NON_EXISTING_RECIPE_ID);
    });
  });
});
