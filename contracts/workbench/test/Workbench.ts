import { loadFixture } from "@nomicfoundation/hardhat-network-helpers";
import { expect } from "chai";
import { ethers } from "hardhat";

describe("Workbench", function () {
  const COLLECTION_ADDRESS = "0xd9145CCE52D386f254917e481eB44e9943F39138";

  async function deployWorkbenchFixture() {
    const [owner, ...otherAccounts] = await ethers.getSigners();

    const Workbench = await ethers.getContractFactory("MyWorkbench");
    const workbench = await Workbench.deploy(COLLECTION_ADDRESS);

    return { workbench, owner, otherAccounts };
  }

  describe("Deployment", function () {
    it("should set a collection address", async function () {
      const { workbench } = await loadFixture(deployWorkbenchFixture);

      expect(workbench.collection).not.to.equal(COLLECTION_ADDRESS);
    });
  });

  const NON_EXISTING_BLUEPRINT_ID = 12345n;
  const BLUEPRINTS = {
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

  describe("Hash Blueprint Function", function () {
    it("should hash the blueprint correctly", async function () {
      const { workbench } = await loadFixture(deployWorkbenchFixture);
      const { inputIds, inputAmounts, outputIds, outputAmounts } =
        BLUEPRINTS.valid;

      const expectedBlueprintId = ethers.BigNumber.from(
        ethers.utils.keccak256(
          ethers.utils.defaultAbiCoder.encode(
            ["uint256[]", "uint256[]", "uint256[]", "uint256[]"],
            [inputIds, inputAmounts, outputIds, outputAmounts]
          )
        )
      );
      const actualBlueprintId = await workbench.hashBlueprint(
        inputIds,
        inputAmounts,
        outputIds,
        outputAmounts
      );

      expect(actualBlueprintId).to.equal(expectedBlueprintId);
    });
  });

  describe("Create Blueprint Function", function () {
    it("should revert in case of empty array as a parameter", async function () {
      const { workbench } = await loadFixture(deployWorkbenchFixture);

      for (const scenario of BLUEPRINTS.empty) {
        const { inputIds, inputAmounts, outputIds, outputAmounts } = scenario;
        await expect(
          workbench.createBlueprint(
            inputIds,
            inputAmounts,
            outputIds,
            outputAmounts
          )
        ).to.be.revertedWithCustomError(workbench, "InvalidBlueprintParams");
      }
    });

    it("should revert if there is a mismatch between the ids and amounts length", async function () {
      const { workbench } = await loadFixture(deployWorkbenchFixture);

      for (const scenario of BLUEPRINTS.mismatch) {
        const { inputIds, inputAmounts, outputIds, outputAmounts } = scenario;
        await expect(
          workbench.createBlueprint(
            inputIds,
            inputAmounts,
            outputIds,
            outputAmounts
          )
        ).to.be.revertedWithCustomError(workbench, "InvalidBlueprintParams");
      }
    });

    it("should revert if input or output amount is zero", async function () {
      const { workbench } = await loadFixture(deployWorkbenchFixture);

      for (const scenario of BLUEPRINTS.zeroAmount) {
        const { inputIds, inputAmounts, outputIds, outputAmounts } = scenario;
        await expect(
          workbench.createBlueprint(
            inputIds,
            inputAmounts,
            outputIds,
            outputAmounts
          )
        ).to.be.revertedWithCustomError(workbench, "InvalidBlueprintParams");
      }
    });

    it("should create a new blueprint with valid parameters", async function () {
      const { workbench } = await loadFixture(deployWorkbenchFixture);
      const { inputIds, inputAmounts, outputIds, outputAmounts } =
        BLUEPRINTS.valid;

      const expectedBlueprintId = await workbench.hashBlueprint(
        inputIds,
        inputAmounts,
        outputIds,
        outputAmounts
      );

      await expect(
        workbench.createBlueprint(
          inputIds,
          inputAmounts,
          outputIds,
          outputAmounts
        )
      )
        .to.emit(workbench, "BlueprintCreated")
        .withArgs(expectedBlueprintId);
    });
  });

  describe("Delete Blueprint Function", function () {
    it("should revert if trying to delete a non-existing blueprint", async function () {
      const { workbench } = await loadFixture(deployWorkbenchFixture);

      await expect(workbench.deleteBlueprint(NON_EXISTING_BLUEPRINT_ID))
        .to.be.revertedWithCustomError(workbench, "BlueprintNotFound")
        .withArgs(NON_EXISTING_BLUEPRINT_ID);
    });

    it("should revert if non-owner tries to delete a blueprint", async function () {
      const { workbench, otherAccounts } = await loadFixture(
        deployWorkbenchFixture
      );
      const { inputIds, inputAmounts, outputIds, outputAmounts } =
        BLUEPRINTS.valid;

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
        workbench.connect(otherAccounts[0]).deleteBlueprint(blueprintId)
      ).to.be.reverted;
    });

    it("should allow owner to delete the existing blueprint", async function () {
      const { workbench } = await loadFixture(deployWorkbenchFixture);
      const { inputIds, inputAmounts, outputIds, outputAmounts } =
        BLUEPRINTS.valid;

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

      await expect(workbench.deleteBlueprint(blueprintId))
        .to.emit(workbench, "BlueprintDeleted")
        .withArgs(blueprintId);
    });
  });

  describe("Craft Function", function () {
    it("should revert if trying to craft with a non-existing blueprint", async function () {
      const { workbench, otherAccounts } = await loadFixture(
        deployWorkbenchFixture
      );

      await expect(
        workbench.connect(otherAccounts[0]).craft(NON_EXISTING_BLUEPRINT_ID)
      )
        .to.be.revertedWithCustomError(workbench, "BlueprintNotFound")
        .withArgs(NON_EXISTING_BLUEPRINT_ID);
    });
  });
});
