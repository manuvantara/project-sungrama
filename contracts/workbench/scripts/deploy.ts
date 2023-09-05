import { ethers } from "hardhat";

async function main() {
  const Collection = await ethers.getContractFactory("MyCollection");
  const collection = await Collection.deploy();
  await collection.deployed();

  const Workbench = await ethers.getContractFactory("MyWorkbench");
  const workbench = await Workbench.deploy(collection.address);
  await workbench.deployed();

  console.log(
    `Collection deployed to ${collection.address}. 
    Workbench deployed to ${workbench.address}.`
  );
}

// We recommend this pattern to be able to use async/await everywhere
// and properly handle errors.
main().catch((error) => {
  console.error(error);
  process.exitCode = 1;
});
