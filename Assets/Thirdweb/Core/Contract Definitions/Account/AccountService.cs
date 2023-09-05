using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Numerics;
using Nethereum.Hex.HexTypes;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Web3;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Contracts.CQS;
using Nethereum.Contracts.ContractHandlers;
using Nethereum.Contracts;
using System.Threading;
using Thirdweb.Contracts.Account.ContractDefinition;

namespace Thirdweb.Contracts.Account
{
    public partial class AccountService
    {
        public static Task<TransactionReceipt> DeployContractAndWaitForReceiptAsync(
            Nethereum.Web3.Web3 web3,
            AccountDeployment accountDeployment,
            CancellationTokenSource cancellationTokenSource = null
        )
        {
            return web3.Eth.GetContractDeploymentHandler<AccountDeployment>().SendRequestAndWaitForReceiptAsync(accountDeployment, cancellationTokenSource);
        }

        public static Task<string> DeployContractAsync(Nethereum.Web3.Web3 web3, AccountDeployment accountDeployment)
        {
            return web3.Eth.GetContractDeploymentHandler<AccountDeployment>().SendRequestAsync(accountDeployment);
        }

        public static async Task<AccountService> DeployContractAndGetServiceAsync(Nethereum.Web3.Web3 web3, AccountDeployment accountDeployment, CancellationTokenSource cancellationTokenSource = null)
        {
            var receipt = await DeployContractAndWaitForReceiptAsync(web3, accountDeployment, cancellationTokenSource);
            return new AccountService(web3, receipt.ContractAddress);
        }

        protected Nethereum.Web3.IWeb3 Web3 { get; }

        public ContractHandler ContractHandler { get; }

        public AccountService(Nethereum.Web3.Web3 web3, string contractAddress)
        {
            Web3 = web3;
            ContractHandler = web3.Eth.GetContractHandler(contractAddress);
        }

        public AccountService(Nethereum.Web3.IWeb3 web3, string contractAddress)
        {
            Web3 = web3;
            ContractHandler = web3.Eth.GetContractHandler(contractAddress);
        }

        public Task<string> AddDepositRequestAsync(AddDepositFunction addDepositFunction)
        {
            return ContractHandler.SendRequestAsync(addDepositFunction);
        }

        public Task<string> AddDepositRequestAsync()
        {
            return ContractHandler.SendRequestAsync<AddDepositFunction>();
        }

        public Task<TransactionReceipt> AddDepositRequestAndWaitForReceiptAsync(AddDepositFunction addDepositFunction, CancellationTokenSource cancellationToken = null)
        {
            return ContractHandler.SendRequestAndWaitForReceiptAsync(addDepositFunction, cancellationToken);
        }

        public Task<TransactionReceipt> AddDepositRequestAndWaitForReceiptAsync(CancellationTokenSource cancellationToken = null)
        {
            return ContractHandler.SendRequestAndWaitForReceiptAsync<AddDepositFunction>(null, cancellationToken);
        }

        public Task<string> ChangeRoleRequestAsync(ChangeRoleFunction changeRoleFunction)
        {
            return ContractHandler.SendRequestAsync(changeRoleFunction);
        }

        public Task<TransactionReceipt> ChangeRoleRequestAndWaitForReceiptAsync(ChangeRoleFunction changeRoleFunction, CancellationTokenSource cancellationToken = null)
        {
            return ContractHandler.SendRequestAndWaitForReceiptAsync(changeRoleFunction, cancellationToken);
        }

        public Task<string> ChangeRoleRequestAsync(RoleRequest req, byte[] signature)
        {
            var changeRoleFunction = new ChangeRoleFunction();
            changeRoleFunction.Req = req;
            changeRoleFunction.Signature = signature;

            return ContractHandler.SendRequestAsync(changeRoleFunction);
        }

        public Task<TransactionReceipt> ChangeRoleRequestAndWaitForReceiptAsync(RoleRequest req, byte[] signature, CancellationTokenSource cancellationToken = null)
        {
            var changeRoleFunction = new ChangeRoleFunction();
            changeRoleFunction.Req = req;
            changeRoleFunction.Signature = signature;

            return ContractHandler.SendRequestAndWaitForReceiptAsync(changeRoleFunction, cancellationToken);
        }

        public Task<string> ContractURIQueryAsync(ContractURIFunction contractURIFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<ContractURIFunction, string>(contractURIFunction, blockParameter);
        }

        public Task<string> ContractURIQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<ContractURIFunction, string>(null, blockParameter);
        }

        public Task<string> EntryPointQueryAsync(EntryPointFunction entryPointFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<EntryPointFunction, string>(entryPointFunction, blockParameter);
        }

        public Task<string> EntryPointQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<EntryPointFunction, string>(null, blockParameter);
        }

        public Task<string> ExecuteRequestAsync(ExecuteFunction executeFunction)
        {
            return ContractHandler.SendRequestAsync(executeFunction);
        }

        public Task<TransactionReceipt> ExecuteRequestAndWaitForReceiptAsync(ExecuteFunction executeFunction, CancellationTokenSource cancellationToken = null)
        {
            return ContractHandler.SendRequestAndWaitForReceiptAsync(executeFunction, cancellationToken);
        }

        public Task<string> ExecuteRequestAsync(string target, BigInteger value, byte[] calldata)
        {
            var executeFunction = new ExecuteFunction();
            executeFunction.Target = target;
            executeFunction.Value = value;
            executeFunction.Calldata = calldata;

            return ContractHandler.SendRequestAsync(executeFunction);
        }

        public Task<TransactionReceipt> ExecuteRequestAndWaitForReceiptAsync(string target, BigInteger value, byte[] calldata, CancellationTokenSource cancellationToken = null)
        {
            var executeFunction = new ExecuteFunction();
            executeFunction.Target = target;
            executeFunction.Value = value;
            executeFunction.Calldata = calldata;

            return ContractHandler.SendRequestAndWaitForReceiptAsync(executeFunction, cancellationToken);
        }

        public Task<string> ExecuteBatchRequestAsync(ExecuteBatchFunction executeBatchFunction)
        {
            return ContractHandler.SendRequestAsync(executeBatchFunction);
        }

        public Task<TransactionReceipt> ExecuteBatchRequestAndWaitForReceiptAsync(ExecuteBatchFunction executeBatchFunction, CancellationTokenSource cancellationToken = null)
        {
            return ContractHandler.SendRequestAndWaitForReceiptAsync(executeBatchFunction, cancellationToken);
        }

        public Task<string> ExecuteBatchRequestAsync(List<string> target, List<BigInteger> value, List<byte[]> calldata)
        {
            var executeBatchFunction = new ExecuteBatchFunction();
            executeBatchFunction.Target = target;
            executeBatchFunction.Value = value;
            executeBatchFunction.Calldata = calldata;

            return ContractHandler.SendRequestAsync(executeBatchFunction);
        }

        public Task<TransactionReceipt> ExecuteBatchRequestAndWaitForReceiptAsync(List<string> target, List<BigInteger> value, List<byte[]> calldata, CancellationTokenSource cancellationToken = null)
        {
            var executeBatchFunction = new ExecuteBatchFunction();
            executeBatchFunction.Target = target;
            executeBatchFunction.Value = value;
            executeBatchFunction.Calldata = calldata;

            return ContractHandler.SendRequestAndWaitForReceiptAsync(executeBatchFunction, cancellationToken);
        }

        public Task<string> FactoryQueryAsync(FactoryFunction factoryFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<FactoryFunction, string>(factoryFunction, blockParameter);
        }

        public Task<string> FactoryQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<FactoryFunction, string>(null, blockParameter);
        }

        public Task<List<string>> GetAllRoleMembersQueryAsync(GetAllRoleMembersFunction getAllRoleMembersFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<GetAllRoleMembersFunction, List<string>>(getAllRoleMembersFunction, blockParameter);
        }

        public Task<List<string>> GetAllRoleMembersQueryAsync(byte[] role, BlockParameter blockParameter = null)
        {
            var getAllRoleMembersFunction = new GetAllRoleMembersFunction();
            getAllRoleMembersFunction.Role = role;

            return ContractHandler.QueryAsync<GetAllRoleMembersFunction, List<string>>(getAllRoleMembersFunction, blockParameter);
        }

        public Task<BigInteger> GetDepositQueryAsync(GetDepositFunction getDepositFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<GetDepositFunction, BigInteger>(getDepositFunction, blockParameter);
        }

        public Task<BigInteger> GetDepositQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<GetDepositFunction, BigInteger>(null, blockParameter);
        }

        public Task<BigInteger> GetNonceQueryAsync(GetNonceFunction getNonceFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<GetNonceFunction, BigInteger>(getNonceFunction, blockParameter);
        }

        public Task<BigInteger> GetNonceQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<GetNonceFunction, BigInteger>(null, blockParameter);
        }

        public Task<GetRoleRestrictionsOutputDTO> GetRoleRestrictionsQueryAsync(GetRoleRestrictionsFunction getRoleRestrictionsFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryDeserializingToObjectAsync<GetRoleRestrictionsFunction, GetRoleRestrictionsOutputDTO>(getRoleRestrictionsFunction, blockParameter);
        }

        public Task<GetRoleRestrictionsOutputDTO> GetRoleRestrictionsQueryAsync(byte[] role, BlockParameter blockParameter = null)
        {
            var getRoleRestrictionsFunction = new GetRoleRestrictionsFunction();
            getRoleRestrictionsFunction.Role = role;

            return ContractHandler.QueryDeserializingToObjectAsync<GetRoleRestrictionsFunction, GetRoleRestrictionsOutputDTO>(getRoleRestrictionsFunction, blockParameter);
        }

        public Task<GetRoleRestrictionsForAccountOutputDTO> GetRoleRestrictionsForAccountQueryAsync(
            GetRoleRestrictionsForAccountFunction getRoleRestrictionsForAccountFunction,
            BlockParameter blockParameter = null
        )
        {
            return ContractHandler.QueryDeserializingToObjectAsync<GetRoleRestrictionsForAccountFunction, GetRoleRestrictionsForAccountOutputDTO>(
                getRoleRestrictionsForAccountFunction,
                blockParameter
            );
        }

        public Task<GetRoleRestrictionsForAccountOutputDTO> GetRoleRestrictionsForAccountQueryAsync(string account, BlockParameter blockParameter = null)
        {
            var getRoleRestrictionsForAccountFunction = new GetRoleRestrictionsForAccountFunction();
            getRoleRestrictionsForAccountFunction.Account = account;

            return ContractHandler.QueryDeserializingToObjectAsync<GetRoleRestrictionsForAccountFunction, GetRoleRestrictionsForAccountOutputDTO>(
                getRoleRestrictionsForAccountFunction,
                blockParameter
            );
        }

        public Task<string> InitializeRequestAsync(InitializeFunction initializeFunction)
        {
            return ContractHandler.SendRequestAsync(initializeFunction);
        }

        public Task<TransactionReceipt> InitializeRequestAndWaitForReceiptAsync(InitializeFunction initializeFunction, CancellationTokenSource cancellationToken = null)
        {
            return ContractHandler.SendRequestAndWaitForReceiptAsync(initializeFunction, cancellationToken);
        }

        public Task<string> InitializeRequestAsync(string defaultAdmin, byte[] returnValue2)
        {
            var initializeFunction = new InitializeFunction();
            initializeFunction.DefaultAdmin = defaultAdmin;
            initializeFunction.ReturnValue2 = returnValue2;

            return ContractHandler.SendRequestAsync(initializeFunction);
        }

        public Task<TransactionReceipt> InitializeRequestAndWaitForReceiptAsync(string defaultAdmin, byte[] returnValue2, CancellationTokenSource cancellationToken = null)
        {
            var initializeFunction = new InitializeFunction();
            initializeFunction.DefaultAdmin = defaultAdmin;
            initializeFunction.ReturnValue2 = returnValue2;

            return ContractHandler.SendRequestAndWaitForReceiptAsync(initializeFunction, cancellationToken);
        }

        public Task<bool> IsAdminQueryAsync(IsAdminFunction isAdminFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<IsAdminFunction, bool>(isAdminFunction, blockParameter);
        }

        public Task<bool> IsAdminQueryAsync(string account, BlockParameter blockParameter = null)
        {
            var isAdminFunction = new IsAdminFunction();
            isAdminFunction.Account = account;

            return ContractHandler.QueryAsync<IsAdminFunction, bool>(isAdminFunction, blockParameter);
        }

        public Task<byte[]> IsValidSignatureQueryAsync(IsValidSignatureFunction isValidSignatureFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<IsValidSignatureFunction, byte[]>(isValidSignatureFunction, blockParameter);
        }

        public Task<byte[]> IsValidSignatureQueryAsync(byte[] hash, byte[] signature, BlockParameter blockParameter = null)
        {
            var isValidSignatureFunction = new IsValidSignatureFunction();
            isValidSignatureFunction.Hash = hash;
            isValidSignatureFunction.Signature = signature;

            return ContractHandler.QueryAsync<IsValidSignatureFunction, byte[]>(isValidSignatureFunction, blockParameter);
        }

        public Task<bool> IsValidSignerQueryAsync(IsValidSignerFunction isValidSignerFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<IsValidSignerFunction, bool>(isValidSignerFunction, blockParameter);
        }

        public Task<bool> IsValidSignerQueryAsync(string signer, UserOperation userOp, BlockParameter blockParameter = null)
        {
            var isValidSignerFunction = new IsValidSignerFunction();
            isValidSignerFunction.Signer = signer;
            isValidSignerFunction.UserOp = userOp;

            return ContractHandler.QueryAsync<IsValidSignerFunction, bool>(isValidSignerFunction, blockParameter);
        }

        public Task<string> MulticallRequestAsync(MulticallFunction multicallFunction)
        {
            return ContractHandler.SendRequestAsync(multicallFunction);
        }

        public Task<TransactionReceipt> MulticallRequestAndWaitForReceiptAsync(MulticallFunction multicallFunction, CancellationTokenSource cancellationToken = null)
        {
            return ContractHandler.SendRequestAndWaitForReceiptAsync(multicallFunction, cancellationToken);
        }

        public Task<string> MulticallRequestAsync(List<byte[]> data)
        {
            var multicallFunction = new MulticallFunction();
            multicallFunction.Data = data;

            return ContractHandler.SendRequestAsync(multicallFunction);
        }

        public Task<TransactionReceipt> MulticallRequestAndWaitForReceiptAsync(List<byte[]> data, CancellationTokenSource cancellationToken = null)
        {
            var multicallFunction = new MulticallFunction();
            multicallFunction.Data = data;

            return ContractHandler.SendRequestAndWaitForReceiptAsync(multicallFunction, cancellationToken);
        }

        public Task<string> OnERC1155BatchReceivedRequestAsync(OnERC1155BatchReceivedFunction onERC1155BatchReceivedFunction)
        {
            return ContractHandler.SendRequestAsync(onERC1155BatchReceivedFunction);
        }

        public Task<TransactionReceipt> OnERC1155BatchReceivedRequestAndWaitForReceiptAsync(
            OnERC1155BatchReceivedFunction onERC1155BatchReceivedFunction,
            CancellationTokenSource cancellationToken = null
        )
        {
            return ContractHandler.SendRequestAndWaitForReceiptAsync(onERC1155BatchReceivedFunction, cancellationToken);
        }

        public Task<string> OnERC1155BatchReceivedRequestAsync(string returnValue1, string returnValue2, List<BigInteger> returnValue3, List<BigInteger> returnValue4, byte[] returnValue5)
        {
            var onERC1155BatchReceivedFunction = new OnERC1155BatchReceivedFunction();
            onERC1155BatchReceivedFunction.ReturnValue1 = returnValue1;
            onERC1155BatchReceivedFunction.ReturnValue2 = returnValue2;
            onERC1155BatchReceivedFunction.ReturnValue3 = returnValue3;
            onERC1155BatchReceivedFunction.ReturnValue4 = returnValue4;
            onERC1155BatchReceivedFunction.ReturnValue5 = returnValue5;

            return ContractHandler.SendRequestAsync(onERC1155BatchReceivedFunction);
        }

        public Task<TransactionReceipt> OnERC1155BatchReceivedRequestAndWaitForReceiptAsync(
            string returnValue1,
            string returnValue2,
            List<BigInteger> returnValue3,
            List<BigInteger> returnValue4,
            byte[] returnValue5,
            CancellationTokenSource cancellationToken = null
        )
        {
            var onERC1155BatchReceivedFunction = new OnERC1155BatchReceivedFunction();
            onERC1155BatchReceivedFunction.ReturnValue1 = returnValue1;
            onERC1155BatchReceivedFunction.ReturnValue2 = returnValue2;
            onERC1155BatchReceivedFunction.ReturnValue3 = returnValue3;
            onERC1155BatchReceivedFunction.ReturnValue4 = returnValue4;
            onERC1155BatchReceivedFunction.ReturnValue5 = returnValue5;

            return ContractHandler.SendRequestAndWaitForReceiptAsync(onERC1155BatchReceivedFunction, cancellationToken);
        }

        public Task<string> OnERC1155ReceivedRequestAsync(OnERC1155ReceivedFunction onERC1155ReceivedFunction)
        {
            return ContractHandler.SendRequestAsync(onERC1155ReceivedFunction);
        }

        public Task<TransactionReceipt> OnERC1155ReceivedRequestAndWaitForReceiptAsync(OnERC1155ReceivedFunction onERC1155ReceivedFunction, CancellationTokenSource cancellationToken = null)
        {
            return ContractHandler.SendRequestAndWaitForReceiptAsync(onERC1155ReceivedFunction, cancellationToken);
        }

        public Task<string> OnERC1155ReceivedRequestAsync(string returnValue1, string returnValue2, BigInteger returnValue3, BigInteger returnValue4, byte[] returnValue5)
        {
            var onERC1155ReceivedFunction = new OnERC1155ReceivedFunction();
            onERC1155ReceivedFunction.ReturnValue1 = returnValue1;
            onERC1155ReceivedFunction.ReturnValue2 = returnValue2;
            onERC1155ReceivedFunction.ReturnValue3 = returnValue3;
            onERC1155ReceivedFunction.ReturnValue4 = returnValue4;
            onERC1155ReceivedFunction.ReturnValue5 = returnValue5;

            return ContractHandler.SendRequestAsync(onERC1155ReceivedFunction);
        }

        public Task<TransactionReceipt> OnERC1155ReceivedRequestAndWaitForReceiptAsync(
            string returnValue1,
            string returnValue2,
            BigInteger returnValue3,
            BigInteger returnValue4,
            byte[] returnValue5,
            CancellationTokenSource cancellationToken = null
        )
        {
            var onERC1155ReceivedFunction = new OnERC1155ReceivedFunction();
            onERC1155ReceivedFunction.ReturnValue1 = returnValue1;
            onERC1155ReceivedFunction.ReturnValue2 = returnValue2;
            onERC1155ReceivedFunction.ReturnValue3 = returnValue3;
            onERC1155ReceivedFunction.ReturnValue4 = returnValue4;
            onERC1155ReceivedFunction.ReturnValue5 = returnValue5;

            return ContractHandler.SendRequestAndWaitForReceiptAsync(onERC1155ReceivedFunction, cancellationToken);
        }

        public Task<string> OnERC721ReceivedRequestAsync(OnERC721ReceivedFunction onERC721ReceivedFunction)
        {
            return ContractHandler.SendRequestAsync(onERC721ReceivedFunction);
        }

        public Task<TransactionReceipt> OnERC721ReceivedRequestAndWaitForReceiptAsync(OnERC721ReceivedFunction onERC721ReceivedFunction, CancellationTokenSource cancellationToken = null)
        {
            return ContractHandler.SendRequestAndWaitForReceiptAsync(onERC721ReceivedFunction, cancellationToken);
        }

        public Task<string> OnERC721ReceivedRequestAsync(string returnValue1, string returnValue2, BigInteger returnValue3, byte[] returnValue4)
        {
            var onERC721ReceivedFunction = new OnERC721ReceivedFunction();
            onERC721ReceivedFunction.ReturnValue1 = returnValue1;
            onERC721ReceivedFunction.ReturnValue2 = returnValue2;
            onERC721ReceivedFunction.ReturnValue3 = returnValue3;
            onERC721ReceivedFunction.ReturnValue4 = returnValue4;

            return ContractHandler.SendRequestAsync(onERC721ReceivedFunction);
        }

        public Task<TransactionReceipt> OnERC721ReceivedRequestAndWaitForReceiptAsync(
            string returnValue1,
            string returnValue2,
            BigInteger returnValue3,
            byte[] returnValue4,
            CancellationTokenSource cancellationToken = null
        )
        {
            var onERC721ReceivedFunction = new OnERC721ReceivedFunction();
            onERC721ReceivedFunction.ReturnValue1 = returnValue1;
            onERC721ReceivedFunction.ReturnValue2 = returnValue2;
            onERC721ReceivedFunction.ReturnValue3 = returnValue3;
            onERC721ReceivedFunction.ReturnValue4 = returnValue4;

            return ContractHandler.SendRequestAndWaitForReceiptAsync(onERC721ReceivedFunction, cancellationToken);
        }

        public Task<string> SetAdminRequestAsync(SetAdminFunction setAdminFunction)
        {
            return ContractHandler.SendRequestAsync(setAdminFunction);
        }

        public Task<TransactionReceipt> SetAdminRequestAndWaitForReceiptAsync(SetAdminFunction setAdminFunction, CancellationTokenSource cancellationToken = null)
        {
            return ContractHandler.SendRequestAndWaitForReceiptAsync(setAdminFunction, cancellationToken);
        }

        public Task<string> SetAdminRequestAsync(string account, bool isAdmin)
        {
            var setAdminFunction = new SetAdminFunction();
            setAdminFunction.Account = account;
            setAdminFunction.IsAdmin = isAdmin;

            return ContractHandler.SendRequestAsync(setAdminFunction);
        }

        public Task<TransactionReceipt> SetAdminRequestAndWaitForReceiptAsync(string account, bool isAdmin, CancellationTokenSource cancellationToken = null)
        {
            var setAdminFunction = new SetAdminFunction();
            setAdminFunction.Account = account;
            setAdminFunction.IsAdmin = isAdmin;

            return ContractHandler.SendRequestAndWaitForReceiptAsync(setAdminFunction, cancellationToken);
        }

        public Task<string> SetContractURIRequestAsync(SetContractURIFunction setContractURIFunction)
        {
            return ContractHandler.SendRequestAsync(setContractURIFunction);
        }

        public Task<TransactionReceipt> SetContractURIRequestAndWaitForReceiptAsync(SetContractURIFunction setContractURIFunction, CancellationTokenSource cancellationToken = null)
        {
            return ContractHandler.SendRequestAndWaitForReceiptAsync(setContractURIFunction, cancellationToken);
        }

        public Task<string> SetContractURIRequestAsync(string uri)
        {
            var setContractURIFunction = new SetContractURIFunction();
            setContractURIFunction.Uri = uri;

            return ContractHandler.SendRequestAsync(setContractURIFunction);
        }

        public Task<TransactionReceipt> SetContractURIRequestAndWaitForReceiptAsync(string uri, CancellationTokenSource cancellationToken = null)
        {
            var setContractURIFunction = new SetContractURIFunction();
            setContractURIFunction.Uri = uri;

            return ContractHandler.SendRequestAndWaitForReceiptAsync(setContractURIFunction, cancellationToken);
        }

        public Task<string> SetRoleRestrictionsRequestAsync(SetRoleRestrictionsFunction setRoleRestrictionsFunction)
        {
            return ContractHandler.SendRequestAsync(setRoleRestrictionsFunction);
        }

        public Task<TransactionReceipt> SetRoleRestrictionsRequestAndWaitForReceiptAsync(SetRoleRestrictionsFunction setRoleRestrictionsFunction, CancellationTokenSource cancellationToken = null)
        {
            return ContractHandler.SendRequestAndWaitForReceiptAsync(setRoleRestrictionsFunction, cancellationToken);
        }

        public Task<string> SetRoleRestrictionsRequestAsync(RoleRestrictions restrictions)
        {
            var setRoleRestrictionsFunction = new SetRoleRestrictionsFunction();
            setRoleRestrictionsFunction.Restrictions = restrictions;

            return ContractHandler.SendRequestAsync(setRoleRestrictionsFunction);
        }

        public Task<TransactionReceipt> SetRoleRestrictionsRequestAndWaitForReceiptAsync(RoleRestrictions restrictions, CancellationTokenSource cancellationToken = null)
        {
            var setRoleRestrictionsFunction = new SetRoleRestrictionsFunction();
            setRoleRestrictionsFunction.Restrictions = restrictions;

            return ContractHandler.SendRequestAndWaitForReceiptAsync(setRoleRestrictionsFunction, cancellationToken);
        }

        public Task<bool> SupportsInterfaceQueryAsync(SupportsInterfaceFunction supportsInterfaceFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<SupportsInterfaceFunction, bool>(supportsInterfaceFunction, blockParameter);
        }

        public Task<bool> SupportsInterfaceQueryAsync(byte[] interfaceId, BlockParameter blockParameter = null)
        {
            var supportsInterfaceFunction = new SupportsInterfaceFunction();
            supportsInterfaceFunction.InterfaceId = interfaceId;

            return ContractHandler.QueryAsync<SupportsInterfaceFunction, bool>(supportsInterfaceFunction, blockParameter);
        }

        public Task<string> ValidateUserOpRequestAsync(ValidateUserOpFunction validateUserOpFunction)
        {
            return ContractHandler.SendRequestAsync(validateUserOpFunction);
        }

        public Task<TransactionReceipt> ValidateUserOpRequestAndWaitForReceiptAsync(ValidateUserOpFunction validateUserOpFunction, CancellationTokenSource cancellationToken = null)
        {
            return ContractHandler.SendRequestAndWaitForReceiptAsync(validateUserOpFunction, cancellationToken);
        }

        public Task<string> ValidateUserOpRequestAsync(UserOperation userOp, byte[] userOpHash, BigInteger missingAccountFunds)
        {
            var validateUserOpFunction = new ValidateUserOpFunction();
            validateUserOpFunction.UserOp = userOp;
            validateUserOpFunction.UserOpHash = userOpHash;
            validateUserOpFunction.MissingAccountFunds = missingAccountFunds;

            return ContractHandler.SendRequestAsync(validateUserOpFunction);
        }

        public Task<TransactionReceipt> ValidateUserOpRequestAndWaitForReceiptAsync(
            UserOperation userOp,
            byte[] userOpHash,
            BigInteger missingAccountFunds,
            CancellationTokenSource cancellationToken = null
        )
        {
            var validateUserOpFunction = new ValidateUserOpFunction();
            validateUserOpFunction.UserOp = userOp;
            validateUserOpFunction.UserOpHash = userOpHash;
            validateUserOpFunction.MissingAccountFunds = missingAccountFunds;

            return ContractHandler.SendRequestAndWaitForReceiptAsync(validateUserOpFunction, cancellationToken);
        }

        public Task<VerifyRoleRequestOutputDTO> VerifyRoleRequestQueryAsync(VerifyRoleRequestFunction verifyRoleRequestFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryDeserializingToObjectAsync<VerifyRoleRequestFunction, VerifyRoleRequestOutputDTO>(verifyRoleRequestFunction, blockParameter);
        }

        public Task<VerifyRoleRequestOutputDTO> VerifyRoleRequestQueryAsync(RoleRequest req, byte[] signature, BlockParameter blockParameter = null)
        {
            var verifyRoleRequestFunction = new VerifyRoleRequestFunction();
            verifyRoleRequestFunction.Req = req;
            verifyRoleRequestFunction.Signature = signature;

            return ContractHandler.QueryDeserializingToObjectAsync<VerifyRoleRequestFunction, VerifyRoleRequestOutputDTO>(verifyRoleRequestFunction, blockParameter);
        }

        public Task<string> WithdrawDepositToRequestAsync(WithdrawDepositToFunction withdrawDepositToFunction)
        {
            return ContractHandler.SendRequestAsync(withdrawDepositToFunction);
        }

        public Task<TransactionReceipt> WithdrawDepositToRequestAndWaitForReceiptAsync(WithdrawDepositToFunction withdrawDepositToFunction, CancellationTokenSource cancellationToken = null)
        {
            return ContractHandler.SendRequestAndWaitForReceiptAsync(withdrawDepositToFunction, cancellationToken);
        }

        public Task<string> WithdrawDepositToRequestAsync(string withdrawAddress, BigInteger amount)
        {
            var withdrawDepositToFunction = new WithdrawDepositToFunction();
            withdrawDepositToFunction.WithdrawAddress = withdrawAddress;
            withdrawDepositToFunction.Amount = amount;

            return ContractHandler.SendRequestAsync(withdrawDepositToFunction);
        }

        public Task<TransactionReceipt> WithdrawDepositToRequestAndWaitForReceiptAsync(string withdrawAddress, BigInteger amount, CancellationTokenSource cancellationToken = null)
        {
            var withdrawDepositToFunction = new WithdrawDepositToFunction();
            withdrawDepositToFunction.WithdrawAddress = withdrawAddress;
            withdrawDepositToFunction.Amount = amount;

            return ContractHandler.SendRequestAndWaitForReceiptAsync(withdrawDepositToFunction, cancellationToken);
        }
    }
}
