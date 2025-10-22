# MMN .NET SDK

.NET SDK for interacting with MMN (Mezon Network) blockchain with Zero-Knowledge proof support.

## Installation

```bash
dotnet add package MmnDotNetSdk
```

## Usage

### Initialize Client

```csharp
using MmnDotNetSdk;
using MmnDotNetSdk.Models;

var config = new Config
{
    Endpoint = "https://your-mmn-endpoint.com"
};

using var client = new MmnClient(config);
```

### Health Check

```csharp
var health = await client.CheckHealthAsync();
Console.WriteLine($"Status: {health.Status}");
```

### Add Transaction

```csharp
var tx = new SignedTx
{
    Tx = new Tx
    {
        Type = (int)TxType.Transfer,
        Sender = "sender_address",
        Recipient = "recipient_address",
        Amount = BigInteger.Parse("1000000000000000000"), // 1 token
        Timestamp = (ulong)DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
        TextData = "Hello MMN",
        Nonce = 1,
        ZkProof = "zk_proof_here",
        ZkPub = "zk_public_input_here"
    },
    Sig = "signature_here"
};

var response = await client.AddTxAsync(tx);
if (response.Ok)
{
    Console.WriteLine($"Transaction hash: {response.TxHash}");
}
```

### Get Account Information

```csharp
var account = await client.GetAccountAsync("account_address");
Console.WriteLine($"Balance: {account.Balance}");
Console.WriteLine($"Nonce: {account.Nonce}");
```

### Get Transaction History

```csharp
var history = await client.GetTxHistoryAsync("account_address", limit: 10, offset: 0, filter: 0);
foreach (var tx in history.Transactions)
{
    Console.WriteLine($"Tx: {tx.Sender} -> {tx.Recipient}, Amount: {tx.Amount}");
}
```

### Get Transaction by Hash

```csharp
var txInfo = await client.GetTxByHashAsync("transaction_hash");
Console.WriteLine($"Status: {txInfo.Status}");
```

### Get Current Nonce

```csharp
var nonce = await client.GetCurrentNonceAsync("account_address", "tag");
Console.WriteLine($"Current nonce: {nonce}");
```

### Build and Sign Transaction

```csharp
using MmnDotNetSdk.Utils;

// Build transaction with ZK proof support
var tx = CryptoHelper.BuildTransferTx(
    txType: (int)TxType.Transfer,
    sender: "sender_address",
    recipient: "recipient_address", 
    amount: BigInteger.Parse("1000000000000000000"),
    nonce: 1,
    timestamp: (ulong)DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
    textData: "Hello MMN",
    extraInfo: new Dictionary<string, string> { ["type"] = "transfer" },
    zkProof: "zk_proof_here",
    zkPub: "zk_public_input_here"
);

// Sign transaction with Ed25519
var publicKey = CryptoHelper.Base58Decode("public_key_base58");
var privateKey = new byte[32]; // 32-byte seed
var signedTx = CryptoHelper.SignTx(tx, publicKey, privateKey);

// Verify signature
bool isValid = CryptoHelper.Verify(tx, signedTx.Sig);
```

### Generate Address

```csharp
// Generate address from input string
string address = CryptoHelper.GenerateAddress("user_input_string");
Console.WriteLine($"Generated address: {address}");
```

### Generate Ed25519 Key Pair

```csharp
// Generate new Ed25519 key pair
var (publicKey, privateKeySeed) = CryptoHelper.GenerateEd25519KeyPair();
string publicKeyBase58 = CryptoHelper.Base58Encode(publicKey);
Console.WriteLine($"Public Key: {publicKeyBase58}");
```

### Zero-Knowledge Proof Integration

#### ZkProveClient

Generate ZK proofs for transactions:

```csharp
using MmnDotNetSdk;

// Initialize ZK prove client
var zkClient = new ZkProveClient("http://localhost:8282");

// Generate ZK proof
var proofResponse = await zkClient.GenerateZkProof(
    userId: "12345",
    address: "user_address", 
    ephemeralPk: "ephemeral_public_key",
    jwt: "jwt_token"
);

if (proofResponse?.Data != null)
{
    string zkProof = proofResponse.Data.Proof;
    string zkPub = proofResponse.Data.PublicInput;
    Console.WriteLine($"ZK Proof: {zkProof}");
    Console.WriteLine($"ZK Public Input: {zkPub}");
}
```

#### ZkAccount Generation

Generate accounts with ZK proof support:

```csharp
using MmnDotNetSdk.Tests;

// Generate ZK account (requires ZK service running)
var zkAccount = await ZkAccount.GenerateZkAccount();
Console.WriteLine($"Address: {zkAccount.Address}");
Console.WriteLine($"Public Key: {zkAccount.PublicKey}");
Console.WriteLine($"ZK Proof: {zkAccount.ZkProof}");
Console.WriteLine($"ZK Public Input: {zkAccount.ZkPub}");

// Generate simple key pair account
var keyPairAccount = await ZkAccount.GenerateKeyPairAccount();
Console.WriteLine($"Public Key: {keyPairAccount.PublicKey}");
```

#### JWT Token Generation

Generate JWT tokens for ZK proof authentication:

```csharp
// JWT tokens are automatically generated within ZkAccount.GenerateZkAccount()
// Uses HMAC SHA256 with proper key padding for compatibility
```

## API Reference

### IMmnClient Interface

Main interface defining methods for interacting with MMN network:

- `Task<Mmn.HealthCheckResponse> CheckHealthAsync()` - Health check service
- `Task<Models.AddTxResponse> AddTxAsync(Models.SignedTx tx)` - Add signed transaction
- `Task<Models.Account> GetAccountAsync(string address)` - Get account information
- `Task<Models.TxHistoryResponse> GetTxHistoryAsync(string address, int limit, int offset, int filter)` - Get transaction history
- `Task<Models.TxInfo> GetTxByHashAsync(string txHash)` - Get transaction information by hash
- `Task<ulong> GetCurrentNonceAsync(string address, string tag)` - Get current nonce
- `Task<Mmnpb.TxService.TxServiceClient> SubscribeTransactionStatusAsync()` - Subscribe to transaction status

### MmnClient

Main class implementing `IMmnClient` and `IDisposable`:

```csharp
public class MmnClient : IMmnClient, IDisposable
{
    public MmnClient(Config config) { ... }
    // Implement interface methods
}
```

### Models

#### Core Models
- `Config` - Client configuration (Endpoint)
- `SignedTx` - Signed transaction
- `Account` - Account information (Address, Balance, Nonce)
- `TxInfo` - Detailed transaction information
- `AddTxResponse` - Response when adding transaction (Ok, TxHash)
- `TxHistoryResponse` - Transaction history response (Transactions, Count)
- `HealthCheckResponse` - Health check response

#### Transaction Models
- `Tx` - Basic transaction
- `SignedTx` - Basic signed transaction
- `UserSig` - User signature
- `TxMetaResponse` - Transaction metadata

### Utility Classes

#### ProtoConverter
Convert between .NET models and protobuf messages:

```csharp
// Base58 encoding/decoding
string encoded = ProtoConverter.Base58Encode(bytes);
byte[] decoded = ProtoConverter.Base58Decode(base58String);

// Protobuf conversions
Mmnpb.Tx protoTx = ProtoConverter.ToProtoTx(tx);
Models.Account account = ProtoConverter.FromProtoAccount(protoResponse);
```

#### ValidationHelper
Validation and serialization utilities:

```csharp
// Address validation
ValidationHelper.ValidateAddress(address);

// Amount validation  
ValidationHelper.ValidateAmount(amount);

// JSON serialization/deserialization
string json = ValidationHelper.SerializeTxExtraInfo(data);
Dictionary<string, string> dict = ValidationHelper.DeserializeTxExtraInfo(json);
```

#### CryptoHelper
Cryptographic operations and transaction building:

```csharp
// Transaction building with ZK proof support
Tx tx = CryptoHelper.BuildTransferTx(
    txType: (int)TxType.Transfer,
    sender: "sender_address",
    recipient: "recipient_address",
    amount: BigInteger.Parse("1000000000000000000"),
    nonce: 1,
    timestamp: (ulong)DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
    textData: "Hello MMN",
    extraInfo: new Dictionary<string, string> { ["type"] = "transfer" },
    zkProof: "zk_proof_here",
    zkPub: "zk_public_input_here"
);

// Transaction signing with Ed25519
SignedTx signedTx = CryptoHelper.SignTx(tx, publicKey, privateKeySeed);

// Signature verification
bool isValid = CryptoHelper.Verify(tx, signedTx.Sig);

// Address generation
string address = CryptoHelper.GenerateAddress("input_string");

// Ed25519 key pair generation
var (publicKey, privateKeySeed) = CryptoHelper.GenerateEd25519KeyPair();

// Base58 encoding/decoding
string encoded = CryptoHelper.Base58Encode(bytes);
byte[] decoded = CryptoHelper.Base58Decode(base58String);

// Transaction serialization
byte[] serialized = CryptoHelper.Serialize(tx);
```

## Dependencies

### NuGet Packages
- `Grpc.Net.Client` - gRPC client
- `Grpc.Tools` - gRPC code generation
- `Google.Protobuf` - Protocol buffers
- `SimpleBase` - Base58 encoding/decoding
- `Chaos.NaCl.Core` - Ed25519 cryptography
- `System.IdentityModel.Tokens.Jwt` - JWT token generation and validation
- `System.Text.Json` - JSON serialization
- `System.Security.Cryptography` - SHA256 hashing

## Installation and Build

### Requirements
- .NET 6.0 or higher
- Visual Studio 2022 or VS Code with C# extension

### Build project
```bash
dotnet build
```

### Generate gRPC clients
gRPC clients are automatically generated when building the project from .proto files in the `Proto/` directory.

## Testing

```bash
dotnet test
```

The SDK includes a full test suite in `MmnDotNetSdk.Tests` to ensure functionality works correctly.

## Notes

- This SDK uses gRPC to communicate with the MMN network
- All methods support async/await pattern
- Client implements IDisposable for proper resource management
- Valid MMN endpoint configuration is required for use
- .proto files are used to automatically generate gRPC clients
- Uses Base58 encoding/decoding with SimpleBase package
- Ed25519 cryptography is implemented for signing and verifying transactions
- Zero-Knowledge proof integration for enhanced privacy and security
- JWT token generation with HMAC SHA256 for ZK proof authentication
- Address generation using SHA256 hashing and Base58 encoding
- Comprehensive test suite included for all functionality
- Supports both faucet and regular transfer transactions
- Transaction serialization follows MMN network protocol standards
