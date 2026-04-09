using Common.WebApi.Security;
using System.Security.Cryptography;
using System.Text;

namespace BolivarianoBank.ApiCore.Tests.WebApi;

/// <summary>
/// Pruebas unitarias para RsaSecurity
/// </summary>
public class RsaSecurityTests : BaseTests
{
    private string PublicKeyBase64;
    private string PrivateKeyBase64;
    private const string TestPlainText = "Hello World! This is a test message for RSA encryption.";

    [SetUp]
    public void Setup()
    {
        // Generar par de claves RSA para las pruebas
        using var rsa = new RSACryptoServiceProvider(2048);
        
        // Exportar clave pública
        var publicKeyPem = rsa.ExportRSAPublicKeyPem();
        PublicKeyBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(publicKeyPem));
        
        // Exportar clave privada
        var privateKeyPem = rsa.ExportRSAPrivateKeyPem();
        PrivateKeyBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(privateKeyPem));
    }

    #region Encrypt Tests

    /// <summary>
    /// TRSA_01: Valida encriptación exitosa con clave pública válida
    /// </summary>
    [Test]
    public void TRSA_01_Encrypt_WithValidPublicKey_ReturnsEncryptedString()
    {
        // Act
        var encrypted = RsaSecurity.Encrypt(PublicKeyBase64, TestPlainText);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(encrypted, Is.Not.Null);
            Assert.That(encrypted, Is.Not.Empty);
            Assert.That(encrypted, Is.Not.EqualTo(TestPlainText));
            Assert.That(encrypted, Does.Match("^[A-Za-z0-9+/=]+$")); // Base64 format
        });
    }

    /// <summary>
    /// TRSA_02: Valida encriptación con padding personalizado
    /// </summary>
    [Test]
    public void TRSA_02_Encrypt_WithCustomPadding_ReturnsEncryptedString()
    {
        // Act
        var encrypted = RsaSecurity.Encrypt(PublicKeyBase64, TestPlainText, RSAEncryptionPadding.Pkcs1);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(encrypted, Is.Not.Null);
            Assert.That(encrypted, Is.Not.Empty);
        });
    }

    /// <summary>
    /// TRSA_03: Valida que lanza excepción cuando el texto plano es nulo
    /// </summary>
    [Test]
    public void TRSA_03_Encrypt_WithNullPlainText_ThrowsInvalidOperationException()
    {
        // Act & Assert
        var ex = Assert.Throws<InvalidOperationException>(() =>
            RsaSecurity.Encrypt(PublicKeyBase64, null));
        Assert.That(ex.Message, Does.Contain("Error mientras se intentaba encriptar texto"));
    }

    /// <summary>
    /// TRSA_04: Valida que lanza excepción cuando el texto plano está vacío
    /// </summary>
    [Test]
    public void TRSA_04_Encrypt_WithEmptyPlainText_ThrowsInvalidOperationException()
    {
        // Act & Assert
        var ex = Assert.Throws<InvalidOperationException>(() =>
            RsaSecurity.Encrypt(PublicKeyBase64, string.Empty));
        Assert.That(ex.Message, Does.Contain("Error mientras se intentaba encriptar texto"));
    }

    /// <summary>
    /// TRSA_05: Valida que lanza excepción cuando la clave pública es nula
    /// </summary>
    [Test]
    public void TRSA_05_Encrypt_WithNullPublicKey_ThrowsInvalidOperationException()
    {
        // Act & Assert
        var ex = Assert.Throws<InvalidOperationException>(() =>
            RsaSecurity.Encrypt(null, TestPlainText));
        Assert.That(ex.Message, Does.Contain("Error mientras se intentaba encriptar texto"));
    }

    /// <summary>
    /// TRSA_06: Valida que lanza excepción cuando la clave pública está vacía
    /// </summary>
    [Test]
    public void TRSA_06_Encrypt_WithEmptyPublicKey_ThrowsInvalidOperationException()
    {
        // Act & Assert
        var ex = Assert.Throws<InvalidOperationException>(() =>
            RsaSecurity.Encrypt(string.Empty, TestPlainText));
        Assert.That(ex.Message, Does.Contain("Error mientras se intentaba encriptar texto"));
    }

    /// <summary>
    /// TRSA_07: Valida que lanza excepción con clave pública inválida
    /// </summary>
    [Test]
    public void TRSA_07_Encrypt_WithInvalidPublicKey_ThrowsInvalidOperationException()
    {
        // Arrange
        var invalidKey = Convert.ToBase64String(Encoding.UTF8.GetBytes("Invalid Key"));

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() =>
            RsaSecurity.Encrypt(invalidKey, TestPlainText));
    }

    /// <summary>
    /// TRSA_08: Valida que diferentes textos producen diferentes encriptaciones
    /// </summary>
    [Test]
    public void TRSA_08_Encrypt_DifferentTexts_ProduceDifferentEncryptions()
    {
        // Arrange
        var text1 = "First message";
        var text2 = "Second message";

        // Act
        var encrypted1 = RsaSecurity.Encrypt(PublicKeyBase64, text1);
        var encrypted2 = RsaSecurity.Encrypt(PublicKeyBase64, text2);

        // Assert
        Assert.That(encrypted1, Is.Not.EqualTo(encrypted2));
    }

    #endregion

    #region Decrypt Tests

    /// <summary>
    /// TRSA_09: Valida desencriptación exitosa de texto encriptado
    /// </summary>
    [Test]
    public void TRSA_09_Decrypt_WithValidEncryptedData_ReturnsOriginalText()
    {
        // Arrange
        var encrypted = RsaSecurity.Encrypt(PublicKeyBase64, TestPlainText);

        // Act
        var decrypted = RsaSecurity.Decrypt(PrivateKeyBase64, encrypted);

        // Assert
        Assert.That(decrypted, Is.EqualTo(TestPlainText));
    }

    /// <summary>
    /// TRSA_10: Valida desencriptación con padding personalizado
    /// </summary>
    [Test]
    public void TRSA_10_Decrypt_WithCustomPadding_ReturnsOriginalText()
    {
        // Arrange
        var encrypted = RsaSecurity.Encrypt(PublicKeyBase64, TestPlainText, RSAEncryptionPadding.Pkcs1);

        // Act
        var decrypted = RsaSecurity.Decrypt(PrivateKeyBase64, encrypted, RSAEncryptionPadding.Pkcs1);

        // Assert
        Assert.That(decrypted, Is.EqualTo(TestPlainText));
    }

    /// <summary>
    /// TRSA_11: Valida que lanza excepción cuando los datos encriptados son nulos
    /// </summary>
    [Test]
    public void TRSA_11_Decrypt_WithNullData_ThrowsInvalidOperationException()
    {
        // Act & Assert
        var ex = Assert.Throws<InvalidOperationException>(() => 
            RsaSecurity.Decrypt(PrivateKeyBase64, null));
        Assert.That(ex.Message, Does.Contain("Error mientras se intentaba desencriptar texto"));
    }

    /// <summary>
    /// TRSA_12: Valida que lanza excepción cuando los datos encriptados están vacíos
    /// </summary>
    [Test]
    public void TRSA_12_Decrypt_WithEmptyData_ThrowsInvalidOperationException()
    {
        // Act & Assert
        var ex = Assert.Throws<InvalidOperationException>(() => 
            RsaSecurity.Decrypt(PrivateKeyBase64, string.Empty));
        Assert.That(ex.Message, Does.Contain("Error mientras se intentaba desencriptar texto"));
    }

    /// <summary>
    /// TRSA_13: Valida que lanza excepción cuando la clave privada es nula
    /// </summary>
    [Test]
    public void TRSA_13_Decrypt_WithNullPrivateKey_ThrowsInvalidOperationException()
    {
        // Arrange
        var encrypted = RsaSecurity.Encrypt(PublicKeyBase64, TestPlainText);

        // Act & Assert
        var ex = Assert.Throws<InvalidOperationException>(() => 
            RsaSecurity.Decrypt(null, encrypted));
        Assert.That(ex.Message, Does.Contain("Error mientras se intentaba desencriptar texto"));
    }

    /// <summary>
    /// TRSA_14: Valida que lanza excepción cuando la clave privada está vacía
    /// </summary>
    [Test]
    public void TRSA_14_Decrypt_WithEmptyPrivateKey_ThrowsInvalidOperationException()
    {
        // Arrange
        var encrypted = RsaSecurity.Encrypt(PublicKeyBase64, TestPlainText);

        // Act & Assert
        var ex = Assert.Throws<InvalidOperationException>(() => 
            RsaSecurity.Decrypt(string.Empty, encrypted));
        Assert.That(ex.Message, Does.Contain("Error mientras se intentaba desencriptar texto"));
    }

    /// <summary>
    /// TRSA_15: Valida que lanza excepción con datos inválidos (no Base64)
    /// </summary>
    [Test]
    public void TRSA_15_Decrypt_WithInvalidBase64Data_ThrowsInvalidOperationException()
    {
        // Arrange
        var invalidData = "This is not base64!@#$%";

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => 
            RsaSecurity.Decrypt(PrivateKeyBase64, invalidData));
    }

    #endregion

    #region SignData Tests

    /// <summary>
    /// TRSA_16: Valida firma de datos con clave privada
    /// </summary>
    [Test]
    public void TRSA_16_SignData_WithValidPrivateKey_ReturnsSignature()
    {
        // Act
        var signature = RsaSecurity.SignData(PrivateKeyBase64, TestPlainText, 
            HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(signature, Is.Not.Null);
            Assert.That(signature, Is.Not.Empty);
            Assert.That(signature, Does.Match("^[A-Za-z0-9+/=]+$")); // Base64 format
        });
    }

    /// <summary>
    /// TRSA_17: Valida firma con diferentes algoritmos de hash
    /// </summary>
    [Test]
    public void TRSA_17_SignData_WithDifferentHashAlgorithms_ProducesDifferentSignatures()
    {
        // Act
        var signatureSHA256 = RsaSecurity.SignData(PrivateKeyBase64, TestPlainText, 
            HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        var signatureSHA512 = RsaSecurity.SignData(PrivateKeyBase64, TestPlainText, 
            HashAlgorithmName.SHA512, RSASignaturePadding.Pkcs1);

        // Assert
        Assert.That(signatureSHA256, Is.Not.EqualTo(signatureSHA512));
    }

    /// <summary>
    /// TRSA_18: Valida que lanza excepción cuando la clave privada es nula para firma
    /// </summary>
    [Test]
    public void TRSA_18_SignData_WithNullPrivateKey_ThrowsInvalidOperationException()
    {
        // Act & Assert
        var ex = Assert.Throws<InvalidOperationException>(() => 
            RsaSecurity.SignData(null, TestPlainText, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1));
        Assert.That(ex.Message, Does.Contain("Error mientras se intentaba firmar texto"));
    }

    /// <summary>
    /// TRSA_19: Valida que diferentes datos producen diferentes firmas
    /// </summary>
    [Test]
    public void TRSA_19_SignData_DifferentData_ProducesDifferentSignatures()
    {
        // Arrange
        var data1 = "First data";
        var data2 = "Second data";

        // Act
        var signature1 = RsaSecurity.SignData(PrivateKeyBase64, data1, 
            HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        var signature2 = RsaSecurity.SignData(PrivateKeyBase64, data2, 
            HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

        // Assert
        Assert.That(signature1, Is.Not.EqualTo(signature2));
    }

    #endregion

    #region VerifySign Tests

    /// <summary>
    /// TRSA_20: Valida verificación exitosa de firma válida
    /// </summary>
    [Test]
    public void TRSA_20_VerifySign_WithValidSignature_ReturnsTrue()
    {
        // Arrange
        var signature = RsaSecurity.SignData(PrivateKeyBase64, TestPlainText, 
            HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

        // Act
        var isValid = RsaSecurity.VerifySign(PublicKeyBase64, TestPlainText, signature, 
            HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

        // Assert
        Assert.That(isValid, Is.True);
    }

    /// <summary>
    /// TRSA_21: Valida que firma inválida retorna false
    /// </summary>
    [Test]
    public void TRSA_21_VerifySign_WithInvalidSignature_ReturnsFalse()
    {
        // Arrange
        var signature = RsaSecurity.SignData(PrivateKeyBase64, TestPlainText, 
            HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        var tamperedData = TestPlainText + " tampered";

        // Act
        var isValid = RsaSecurity.VerifySign(PublicKeyBase64, tamperedData, signature, 
            HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

        // Assert
        Assert.That(isValid, Is.False);
    }

    /// <summary>
    /// TRSA_22: Valida que firma con algoritmo diferente retorna false
    /// </summary>
    [Test]
    public void TRSA_22_VerifySign_WithDifferentHashAlgorithm_ReturnsFalse()
    {
        // Arrange
        var signature = RsaSecurity.SignData(PrivateKeyBase64, TestPlainText, 
            HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

        // Act
        var isValid = RsaSecurity.VerifySign(PublicKeyBase64, TestPlainText, signature, 
            HashAlgorithmName.SHA512, RSASignaturePadding.Pkcs1);

        // Assert
        Assert.That(isValid, Is.False);
    }

    /// <summary>
    /// TRSA_23: Valida que lanza excepción cuando la clave pública es nula para verificación
    /// </summary>
    [Test]
    public void TRSA_23_VerifySign_WithNullPublicKey_ThrowsInvalidOperationException()
    {
        // Arrange
        var signature = RsaSecurity.SignData(PrivateKeyBase64, TestPlainText, 
            HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

        // Act & Assert
        var ex = Assert.Throws<InvalidOperationException>(() => 
            RsaSecurity.VerifySign(null, TestPlainText, signature, 
                HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1));
        Assert.That(ex.Message, Does.Contain("Error Validando la Firma del Texto"));
    }

    #endregion

    #region Integration Tests

    /// <summary>
    /// TRSA_24: Valida ciclo completo de encriptación y desencriptación con texto largo
    /// </summary>
    [Test]
    public void TRSA_24_EncryptDecrypt_WithLongText_MaintainsIntegrity()
    {
        // Arrange
        var longText = "This is a longer text to test RSA encryption and decryption capabilities.";

        // Act
        var encrypted = RsaSecurity.Encrypt(PublicKeyBase64, longText);
        var decrypted = RsaSecurity.Decrypt(PrivateKeyBase64, encrypted);

        // Assert
        Assert.That(decrypted, Is.EqualTo(longText));
    }

    /// <summary>
    /// TRSA_25: Valida ciclo completo de firma y verificación
    /// </summary>
    [Test]
    public void TRSA_25_SignAndVerify_CompleteFlow_WorksCorrectly()
    {
        // Arrange
        var dataToSign = "Important data that needs to be signed";

        // Act
        var signature = RsaSecurity.SignData(PrivateKeyBase64, dataToSign, 
            HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        var isValid = RsaSecurity.VerifySign(PublicKeyBase64, dataToSign, signature, 
            HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

        // Assert
        Assert.That(isValid, Is.True);
    }

    /// <summary>
    /// TRSA_26: Valida encriptación y desencriptación con caracteres especiales
    /// </summary>
    [Test]
    public void TRSA_26_EncryptDecrypt_WithSpecialCharacters_MaintainsIntegrity()
    {
        // Arrange
        var specialText = "¡Hola! ¿Cómo estás? @#$%^&*()";

        // Act
        var encrypted = RsaSecurity.Encrypt(PublicKeyBase64, specialText);
        var decrypted = RsaSecurity.Decrypt(PrivateKeyBase64, encrypted);

        // Assert
        Assert.That(decrypted, Is.EqualTo(specialText));
    }

    /// <summary>
    /// TRSA_27: Valida encriptación y desencriptación con JSON
    /// </summary>
    [Test]
    public void TRSA_27_EncryptDecrypt_WithJsonData_MaintainsIntegrity()
    {
        // Arrange
        var jsonText = "{\"name\":\"John\",\"age\":30}";

        // Act
        var encrypted = RsaSecurity.Encrypt(PublicKeyBase64, jsonText);
        var decrypted = RsaSecurity.Decrypt(PrivateKeyBase64, encrypted);

        // Assert
        Assert.That(decrypted, Is.EqualTo(jsonText));
    }

    /// <summary>
    /// TRSA_28: Valida que no se puede desencriptar con clave privada incorrecta
    /// </summary>
    [Test]
    public void TRSA_28_Decrypt_WithWrongPrivateKey_ThrowsException()
    {
        // Arrange
        var encrypted = RsaSecurity.Encrypt(PublicKeyBase64, TestPlainText);
        
        // Generar otro par de claves
        using var rsa = new RSACryptoServiceProvider(2048);
        var wrongPrivateKeyPem = rsa.ExportRSAPrivateKeyPem();
        var wrongPrivateKeyBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(wrongPrivateKeyPem));

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => 
            RsaSecurity.Decrypt(wrongPrivateKeyBase64, encrypted));
    }   

    #endregion
}
