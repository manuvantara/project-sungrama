import { loadFixture } from "@nomicfoundation/hardhat-network-helpers";
import { expect } from "chai";
import { ethers } from "hardhat";

describe("Collection", function () {
  async function deployCollectionFixture() {
    const [owner, workshop, ...otherAccounts] = await ethers.getSigners();

    const Collection = await ethers.getContractFactory("MyCollection");
    const collection = await Collection.deploy();

    return { collection, workshop, owner, otherAccounts };
  }

  describe("Deployment", function () {
    it("should set the default admin role to the deployer", async function () {
      const { collection, owner } = await loadFixture(deployCollectionFixture);

      const DEFAULT_ADMIN_ROLE = await collection.DEFAULT_ADMIN_ROLE();

      expect(await collection.hasRole(DEFAULT_ADMIN_ROLE, owner.address)).to.be
        .true;
    });
  });

  describe("Minting", function () {
    it("should grant the WORKSHOP_ROLE to the workshop", async function () {
      const { collection, workshop } = await loadFixture(
        deployCollectionFixture
      );

      const WORKSHOP_ROLE = await collection.WORKSHOP_ROLE();
      await collection.grantRole(WORKSHOP_ROLE, workshop.address);

      expect(await collection.hasRole(WORKSHOP_ROLE, workshop.address)).to.be
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

    it("should allow onlyMinter (admin or workshop) to mint tokens", async function () {
      const { collection, workshop, owner, otherAccounts } = await loadFixture(
        deployCollectionFixture
      );

      const WORKSHOP_ROLE = await collection.WORKSHOP_ROLE();
      await collection.grantRole(WORKSHOP_ROLE, workshop.address);

      // Try to mint from owner account
      await expect(collection.mint(owner.address, 1, 1, "0x")).to.not.be
        .reverted;

      // Try to mint from workshop
      await expect(
        collection.connect(workshop).mint(workshop.address, 1, 1, "0x")
      ).to.not.be.reverted;
    });
  });
});
