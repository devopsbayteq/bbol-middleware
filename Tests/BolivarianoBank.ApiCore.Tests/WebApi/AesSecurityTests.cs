using Common.WebApi.Security;

namespace BolivarianoBank.ApiCore.Tests.WebApi;

/// <summary>
/// Pruebas unitarias para AesSecurity
/// </summary>
public class AesSecurityTests : BaseTests
{
    private const string ValidKey = "MySecretKey12345"; // 16 caracteres
    private const string ValidIv = "MyInitVector1234"; // 16 caracteres
    private const string TestPlainText = "Hello World! This is a test message.";

    #region EncryptAes Tests

    /// <summary>
    /// TAES_01: Valida encriptación exitosa con clave válida
    /// </summary>
    [Test]
    public void TAES_01_EncryptAes_WithValidKey_ReturnsEncryptedString()
    {
        // Act
        var encrypted = AesSecurity.EncryptAes(TestPlainText, ValidKey);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(encrypted, Is.Not.Null);
            Assert.That(encrypted, Is.Not.Empty);
            Assert.That(encrypted, Is.Not.EqualTo(TestPlainText));
            Assert.That(encrypted, Is.Not.Empty);
        });
    }

    /// <summary>
    /// TAES_02: Valida encriptación con clave e IV válidos
    /// </summary>
    [Test]
    public void TAES_02_EncryptAes_WithValidKeyAndIv_ReturnsEncryptedString()
    {
        // Act
        var encrypted = AesSecurity.EncryptAes(TestPlainText, ValidKey, ValidIv);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(encrypted, Is.Not.Null);
            Assert.That(encrypted, Is.Not.Empty);
            Assert.That(encrypted, Is.Not.EqualTo(TestPlainText));
        });
    }

    /// <summary>
    /// TAES_03: Valida que lanza excepción cuando el texto plano es nulo
    /// </summary>
    [Test]
    public void TAES_03_EncryptAes_WithNullPlainText_ThrowsArgumentException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => AesSecurity.EncryptAes(null, ValidKey));
    }

    /// <summary>
    /// TAES_04: Valida que lanza excepción cuando el texto plano está vacío
    /// </summary>
    [Test]
    public void TAES_04_EncryptAes_WithEmptyPlainText_ThrowsArgumentException()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => AesSecurity.EncryptAes(string.Empty, ValidKey));
    }

    /// <summary>
    /// TAES_05: Valida que lanza excepción cuando la clave es nula
    /// </summary>
    [Test]
    public void TAES_05_EncryptAes_WithNullKey_ThrowsArgumentException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => AesSecurity.EncryptAes(TestPlainText, null));
    }

    /// <summary>
    /// TAES_06: Valida que lanza excepción cuando la clave está vacía
    /// </summary>
    [Test]
    public void TAES_06_EncryptAes_WithEmptyKey_ThrowsArgumentException()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => AesSecurity.EncryptAes(TestPlainText, string.Empty));
    }

    /// <summary>
    /// TAES_07: Valida que lanza excepción cuando la clave es muy corta
    /// </summary>
    [Test]
    public void TAES_07_EncryptAes_WithShortKey_ThrowsArgumentException()
    {
        // Arrange
        var shortKey = "Short"; // Menos de 15 caracteres

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() => AesSecurity.EncryptAes(TestPlainText, shortKey));
        Assert.That(ex.Message, Does.Contain("La llave debe tener al menos 15 caracteres"));
    }

    /// <summary>
    /// TAES_08: Valida que lanza excepción cuando el IV es muy corto
    /// </summary>
    [Test]
    public void TAES_08_EncryptAes_WithShortIv_ThrowsArgumentException()
    {
        // Arrange
        var shortIv = "Short"; // Menos de 15 caracteres

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() => AesSecurity.EncryptAes(TestPlainText, ValidKey, shortIv));
        Assert.That(ex.Message, Does.Contain("El IV debe tener al menos 15 caracteres"));
    }

    /// <summary>
    /// TAES_09: Valida que diferentes textos producen diferentes encriptaciones
    /// </summary>
    [Test]
    public void TAES_09_EncryptAes_DifferentTexts_ProduceDifferentEncryptions()
    {
        // Arrange
        var text1 = "First message";
        var text2 = "Second message";

        // Act
        var encrypted1 = AesSecurity.EncryptAes(text1, ValidKey);
        var encrypted2 = AesSecurity.EncryptAes(text2, ValidKey);

        // Assert
        Assert.That(encrypted1, Is.Not.EqualTo(encrypted2));
    }

    /// <summary>
    /// TAES_10: Valida que el mismo texto con la misma clave produce la misma encriptación
    /// </summary>
    [Test]
    public void TAES_10_EncryptAes_SameTextAndKey_ProducesSameEncryption()
    {
        // Act
        var encrypted1 = AesSecurity.EncryptAes(TestPlainText, ValidKey);
        var encrypted2 = AesSecurity.EncryptAes(TestPlainText, ValidKey);

        // Assert
        Assert.That(encrypted1, Is.EqualTo(encrypted2));
    }

    #endregion

    #region DecryptAes Tests

    /// <summary>
    /// TAES_11: Valida desencriptación exitosa de texto encriptado
    /// </summary>
    [Test]
    public void TAES_11_DecryptAes_WithValidEncryptedData_ReturnsOriginalText()
    {
        // Arrange
        var encrypted = AesSecurity.EncryptAes(TestPlainText, ValidKey);

        // Act
        var decrypted = AesSecurity.DecryptAes(encrypted, ValidKey);

        // Assert
        Assert.That(decrypted, Is.EqualTo(TestPlainText));
    }

    /// <summary>
    /// TAES_12: Valida desencriptación con clave e IV
    /// </summary>
    [Test]
    public void TAES_12_DecryptAes_WithValidKeyAndIv_ReturnsOriginalText()
    {
        // Arrange
        var encrypted = AesSecurity.EncryptAes(TestPlainText, ValidKey, ValidIv);

        // Act
        var decrypted = AesSecurity.DecryptAes(encrypted, ValidKey, ValidIv);

        // Assert
        Assert.That(decrypted, Is.EqualTo(TestPlainText));
    }

    /// <summary>
    /// TAES_13: Valida que lanza excepción cuando los datos encriptados son nulos
    /// </summary>
    [Test]
    public void TAES_13_DecryptAes_WithNullData_ThrowsArgumentException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => AesSecurity.DecryptAes(null, ValidKey));
    }

    /// <summary>
    /// TAES_14: Valida que lanza excepción cuando los datos encriptados están vacíos
    /// </summary>
    [Test]
    public void TAES_14_DecryptAes_WithEmptyData_ThrowsArgumentException()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => AesSecurity.DecryptAes(string.Empty, ValidKey));
    }

    /// <summary>
    /// TAES_15: Valida que lanza excepción cuando la clave de desencriptación es nula
    /// </summary>
    [Test]
    public void TAES_15_DecryptAes_WithNullKey_ThrowsArgumentException()
    {
        // Arrange
        var encrypted = AesSecurity.EncryptAes(TestPlainText, ValidKey);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => AesSecurity.DecryptAes(encrypted, null));
    }

    /// <summary>
    /// TAES_16: Valida que lanza excepción cuando la clave de desencriptación es muy corta
    /// </summary>
    [Test]
    public void TAES_16_DecryptAes_WithShortKey_ThrowsArgumentException()
    {
        // Arrange
        var encrypted = AesSecurity.EncryptAes(TestPlainText, ValidKey);
        var shortKey = "Short";

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() => AesSecurity.DecryptAes(encrypted, shortKey));
        Assert.That(ex.Message, Does.Contain("La llave debe tener al menos 15 caracteres"));
    }

    /// <summary>
    /// TAES_17: Valida que lanza excepción con datos inválidos (no Base64)
    /// </summary>
    [Test]
    public void TAES_17_DecryptAes_WithInvalidBase64Data_ThrowsFormatException()
    {
        // Arrange
        var invalidData = "This is not base64!@#$%";

        // Act & Assert
        Assert.Throws<FormatException>(() => AesSecurity.DecryptAes(invalidData, ValidKey));
    }

    /// <summary>
    /// TAES_18: Valida que lanza excepción con clave incorrecta
    /// </summary>
    [Test]
    public void TAES_18_DecryptAes_WithWrongKey_ThrowsCryptographicException()
    {
        // Arrange
        var encrypted = AesSecurity.EncryptAes(TestPlainText, ValidKey);
        var wrongKey = "WrongKey12345678";

        // Act & Assert
        Assert.Throws<System.Security.Cryptography.CryptographicException>(() => 
            AesSecurity.DecryptAes(encrypted, wrongKey));
    }

    #endregion

    #region EncryptAesToHex Tests

    /// <summary>
    /// TAES_19: Valida encriptación a formato hexadecimal
    /// </summary>
    [Test]
    public void TAES_19_EncryptAesToHex_WithValidData_ReturnsHexString()
    {
        // Act
        var encrypted = AesSecurity.EncryptAesToHex(TestPlainText, ValidKey, ValidIv);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(encrypted, Is.Not.Null);
            Assert.That(encrypted, Is.Not.Empty);
            Assert.That(encrypted, Does.Match("^[0-9A-Fa-f]+$")); // Valida formato hexadecimal
            Assert.That(encrypted.Length % 2, Is.EqualTo(0)); // Longitud par
        });
    }

    /// <summary>
    /// TAES_20: Valida que EncryptAesToHex produce resultado diferente a EncryptAes
    /// </summary>
    [Test]
    public void TAES_20_EncryptAesToHex_ProducesDifferentFormatThanEncryptAes()
    {
        // Act
        var hexEncrypted = AesSecurity.EncryptAesToHex(TestPlainText, ValidKey, ValidIv);
        var base64Encrypted = AesSecurity.EncryptAes(TestPlainText, ValidKey, ValidIv);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(hexEncrypted, Is.Not.EqualTo(base64Encrypted));
            Assert.That(hexEncrypted, Does.Match("^[0-9A-Fa-f]+$")); // Hex
            Assert.That(base64Encrypted, Does.Match("^[A-Za-z0-9+/=]+$")); // Base64
        });
    }

    #endregion

    #region Integration Tests

    /// <summary>
    /// TAES_21: Valida ciclo completo de encriptación y desencriptación con texto largo
    /// </summary>
    [Test]
    public void TAES_21_EncryptDecrypt_WithLongText_MaintainsIntegrity()
    {
        // Arrange
        var longText = string.Join(" ", Enumerable.Repeat("Lorem ipsum dolor sit amet, consectetur adipiscing elit.", 10));

        // Act
        var encrypted = AesSecurity.EncryptAes(longText, ValidKey);
        var decrypted = AesSecurity.DecryptAes(encrypted, ValidKey);

        // Assert
        Assert.That(decrypted, Is.EqualTo(longText));
    }

    /// <summary>
    /// TAES_22: Valida encriptación y desencriptación con caracteres especiales
    /// </summary>
    [Test]
    public void TAES_22_EncryptDecrypt_WithSpecialCharacters_MaintainsIntegrity()
    {
        // Arrange
        var specialText = "¡Hola! ¿Cómo estás? @#$%^&*()_+-=[]{}|;':\",./<>?";

        // Act
        var encrypted = AesSecurity.EncryptAes(specialText, ValidKey);
        var decrypted = AesSecurity.DecryptAes(encrypted, ValidKey);

        // Assert
        Assert.That(decrypted, Is.EqualTo(specialText));
    }

    /// <summary>
    /// TAES_23: Valida encriptación y desencriptación con números
    /// </summary>
    [Test]
    public void TAES_23_EncryptDecrypt_WithNumbers_MaintainsIntegrity()
    {
        // Arrange
        var numberText = "1234567890 9876543210 0.123456789";

        // Act
        var encrypted = AesSecurity.EncryptAes(numberText, ValidKey);
        var decrypted = AesSecurity.DecryptAes(encrypted, ValidKey);

        // Assert
        Assert.That(decrypted, Is.EqualTo(numberText));
    }

    /// <summary>
    /// TAES_24: Valida encriptación y desencriptación con JSON
    /// </summary>
    [Test]
    public void TAES_24_EncryptDecrypt_WithJsonData_MaintainsIntegrity()
    {
        // Arrange
        var jsonText = "{\"name\":\"John Doe\",\"age\":30,\"email\":\"john@example.com\"}";

        // Act
        var encrypted = AesSecurity.EncryptAes(jsonText, ValidKey);
        var decrypted = AesSecurity.DecryptAes(encrypted, ValidKey);

        // Assert
        Assert.That(decrypted, Is.EqualTo(jsonText));
    }

    /// <summary>
    /// TAES_25: Valida que diferentes claves producen diferentes encriptaciones
    /// </summary>
    [Test]
    public void TAES_25_EncryptAes_WithDifferentKeys_ProducesDifferentEncryptions()
    {
        // Arrange
        var key1 = "FirstKey12345678";
        var key2 = "SecondKey1234567";

        // Act
        var encrypted1 = AesSecurity.EncryptAes(TestPlainText, key1);
        var encrypted2 = AesSecurity.EncryptAes(TestPlainText, key2);

        // Assert
        Assert.That(encrypted1, Is.Not.EqualTo(encrypted2));
    }

    #endregion
}
