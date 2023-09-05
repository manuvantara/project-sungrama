import { HardhatUserConfig } from "hardhat/config";
import "@nomicfoundation/hardhat-toolbox";
import "dotenv/config";

const config: HardhatUserConfig = {
  solidity: "0.8.18",
  networks: {
    opBNB_testnet: {
      url: `https://opbnb-testnet-rpc.bnbchain.org/`,
      accounts: [process.env.PRIVATE_KEY!],
      chainId: 5611,
    },
    
    
  }
};

export default config;
