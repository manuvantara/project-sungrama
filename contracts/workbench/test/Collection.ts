import { loadFixture } from "@nomicfoundation/hardhat-network-helpers";
import { expect } from "chai";
import { ethers } from "hardhat";

describe("Collection", function () {
  async function deployCollectionFixture() {
    const [owner, workbench, ...otherAccounts] = await ethers.getSigners();

    const Collection = await ethers.getContractFactory("MyCollection");
    const collection = await Collection.deploy();

    return { collection, workbench, owner, otherAccounts };
  }

  describe("Deployment", function () {
    it("should set the default admin role to the deployer", async function () {
      const { collection, owner } = await loadFixture(deployCollectionFixture);

      const DEFAULT_ADMIN_ROLE = await collection.DEFAULT_ADMIN_ROLE();

      expect(await collection.hasRole(DEFAULT_ADMIN_ROLE, owner.address)).to.be
        .true;
    });
  });

  describe("Minting Access", function () {
    it("should grant the WORKBENCH_ROLE to the workbench", async function () {
      const { collection, workbench } = await loadFixture(
        deployCollectionFixture
      );

      const WORKBENCH_ROLE = await collection.WORKBENCH_ROLE();
      await collection.grantRole(WORKBENCH_ROLE, workbench.address);

      expect(await collection.hasRole(WORKBENCH_ROLE, workbench.address)).to.be
        .true;
    });

    it("should not allow not minter to mint tokens", async function () {
      const { collection, otherAccounts } = await loadFixture(
        deployCollectionFixture
      );

      // Mint from other account (not minter)
      await expect(
        collection
          .connect(otherAccounts[0])
          .mint(otherAccounts[0].address, 1, 1, "0x")
      ).to.be.reverted;
    });

    it("should allow onlyMinter (admin or workbench) to mint tokens", async function () {
      const { collection, workbench, owner } = await loadFixture(
        deployCollectionFixture
      );

      const WORKBENCH_ROLE = await collection.WORKBENCH_ROLE();
      await collection.grantRole(WORKBENCH_ROLE, workbench.address);

      // Try to mint from owner account
      await expect(collection.mint(owner.address, 1, 1, "0x")).to.not.be
        .reverted;

      // Try to mint from workbench
      await expect(
        collection.connect(workbench).mint(workbench.address, 1, 1, "0x")
      ).to.not.be.reverted;
    });
  });
});
