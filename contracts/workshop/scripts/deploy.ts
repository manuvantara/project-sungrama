import { ethers } from "hardhat";

async function main() {


  const Collection = await ethers.getContractFactory("MyCollection");
  const collection = await Collection.deploy();
  await collection.deployed();

  const Workshop = await ethers.getContractFactory("MyWorkshop");
  const workshop = await Workshop.deploy(collection.address);
  await workshop.deployed();

  console.log(
    `Collection deployed to ${collection.address}. 
    Workshop deployed to ${workshop.address}.`
  );
}

// We recommend this pattern to be able to use async/await everywhere
// and properly handle errors.
main().catch((error) => {
  console.error(error);
  process.exitCode = 1;
});
