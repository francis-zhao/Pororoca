using System.Text;
using System.Text.Json;
using Pororoca.Domain.Features.Entities.Pororoca.Http;
using Xunit;
using static Pororoca.Domain.Features.ImportCollection.OpenApiImporter;
using static Pororoca.Domain.Features.Common.JsonConfiguration;

namespace Pororoca.Domain.Tests.Features.ImportCollection;

public static class OpenApiImporterTests
{
    [Fact]
    public static void Should_import_all_requests_from_valid_openapi_without_tags_correctly()
    {
        // GIVEN
        string fileContent = GetTestFile("openapi_imgflip.yml");

        // WHEN AND THEN
        Assert.True(TryImportOpenApi(fileContent, out var col));

        // THEN
        Assert.NotNull(col);
        Assert.Equal("Imgflip API", col.Name);

        #region ENVIRONMENTS

        var env = Assert.Single(col.Environments);

        Assert.Equal("env1", env.Name);
        var baseUrlVar = Assert.Single(env.Variables);
        Assert.True(baseUrlVar.Enabled);
        Assert.Equal("BaseUrl", baseUrlVar.Key);
        Assert.Equal("https://api.imgflip.com", baseUrlVar.Value);

        #endregion

        #region TOTAL REQUESTS, URLS, NAMES AND HTTP METHODS

        Assert.Empty(col.Folders);
        Assert.Equal(4, col.Requests.Count);

        Assert.Equal("Get popular memes", col.HttpRequests[0].Name);
        Assert.Equal("GET", col.HttpRequests[0].HttpMethod);
        Assert.Equal("{{BaseUrl}}/get_memes", col.HttpRequests[0].Url);

        Assert.Equal("Add a caption to an Imgflip meme template", col.HttpRequests[1].Name);
        Assert.Equal("POST", col.HttpRequests[1].HttpMethod);
        Assert.Equal("{{BaseUrl}}/caption_image", col.HttpRequests[1].Url);

        Assert.Equal("Search for meme templates", col.HttpRequests[2].Name);
        Assert.Equal("POST", col.HttpRequests[2].HttpMethod);
        Assert.Equal("{{BaseUrl}}/search_memes", col.HttpRequests[2].Url);

        Assert.Equal("Create a custom meme", col.HttpRequests[3].Name);
        Assert.Equal("POST", col.HttpRequests[3].HttpMethod);
        Assert.Equal("{{BaseUrl}}/create_meme", col.HttpRequests[3].Url);

        #endregion
    }

    [Fact]
    public static void Should_import_all_requests_from_valid_openapi_with_tags_correctly()
    {
        // GIVEN
        string fileContent = GetTestFile("openapi_pix.yaml");

        // WHEN AND THEN
        Assert.True(TryImportOpenApi(fileContent, out var col));

        // THEN
        Assert.NotNull(col);
        Assert.Equal("API Pix", col.Name);

        #region ENVIRONMENTS

        Assert.Equal(2, col.Environments.Count);

        Assert.Equal("Servidor de Produção", col.Environments[0].Name);
        var prodBaseUrlVar = Assert.Single(col.Environments[0].Variables);
        Assert.True(prodBaseUrlVar.Enabled);
        Assert.Equal("BaseUrl", prodBaseUrlVar.Key);
        Assert.Equal("https://pix.example.com/api", prodBaseUrlVar.Value);

        Assert.Equal("Servidor de Homologação", col.Environments[1].Name);
        var hmlBaseUrlVar = Assert.Single(col.Environments[1].Variables);
        Assert.True(hmlBaseUrlVar.Enabled);
        Assert.Equal("BaseUrl", hmlBaseUrlVar.Key);
        Assert.Equal("https://pix-h.example.com/api", hmlBaseUrlVar.Value);

        #endregion

        #region TOTAL REQUESTS, URLS, NAMES AND HTTP METHODS

        Assert.Equal(7, col.Folders.Count);

        //------------------------
        Assert.Equal("Cob", col.Folders[0].Name);

        Assert.Equal("Criar cobrança imediata.", col.Folders[0].Requests[0].Name);
        Assert.Equal("PUT", col.Folders[0].HttpRequests[0].HttpMethod);
        Assert.Equal("{{BaseUrl}}/cob/{{txid}}", col.Folders[0].HttpRequests[0].Url);

        Assert.Equal("Revisar cobrança imediata.", col.Folders[0].Requests[1].Name);
        Assert.Equal("PATCH", col.Folders[0].HttpRequests[1].HttpMethod);
        Assert.Equal("{{BaseUrl}}/cob/{{txid}}", col.Folders[0].HttpRequests[1].Url);

        Assert.Equal("Consultar cobrança imediata.", col.Folders[0].Requests[2].Name);
        Assert.Equal("GET", col.Folders[0].HttpRequests[2].HttpMethod);
        Assert.Equal("{{BaseUrl}}/cob/{{txid}}?revisao=0", col.Folders[0].HttpRequests[2].Url);

        Assert.Equal("Criar cobrança imediata.", col.Folders[0].Requests[3].Name);
        Assert.Equal("POST", col.Folders[0].HttpRequests[3].HttpMethod);
        Assert.Equal("{{BaseUrl}}/cob", col.Folders[0].HttpRequests[3].Url);

        Assert.Equal("Consultar lista de cobranças imediatas.", col.Folders[0].Requests[4].Name);
        Assert.Equal("GET", col.Folders[0].HttpRequests[4].HttpMethod);
        Assert.Contains("{{BaseUrl}}/cob?inicio=", col.Folders[0].HttpRequests[4].Url);
        //------------------------
        Assert.Equal("CobV", col.Folders[1].Name);

        Assert.Equal("Criar cobrança com vencimento.", col.Folders[1].Requests[0].Name);
        Assert.Equal("PUT", col.Folders[1].HttpRequests[0].HttpMethod);
        Assert.Equal("{{BaseUrl}}/cobv/{{txid}}", col.Folders[1].HttpRequests[0].Url);

        Assert.Equal("Revisar cobrança com vencimento.", col.Folders[1].Requests[1].Name);
        Assert.Equal("PATCH", col.Folders[1].HttpRequests[1].HttpMethod);
        Assert.Equal("{{BaseUrl}}/cobv/{{txid}}", col.Folders[1].HttpRequests[1].Url);

        Assert.Equal("Consultar cobrança com vencimento.", col.Folders[1].Requests[2].Name);
        Assert.Equal("GET", col.Folders[1].HttpRequests[2].HttpMethod);
        Assert.Equal("{{BaseUrl}}/cobv/{{txid}}?revisao=0", col.Folders[1].HttpRequests[2].Url);

        Assert.Equal("Consultar lista de cobranças com vencimento.", col.Folders[1].Requests[3].Name);
        Assert.Equal("GET", col.Folders[1].HttpRequests[3].HttpMethod);
        Assert.Contains("{{BaseUrl}}/cobv?inicio=", col.Folders[1].HttpRequests[3].Url);
        //------------------------
        Assert.Equal("LoteCobV", col.Folders[2].Name);

        Assert.Equal("Criar/Alterar lote de cobranças com vencimento.", col.Folders[2].Requests[0].Name);
        Assert.Equal("PUT", col.Folders[2].HttpRequests[0].HttpMethod);
        Assert.Equal("{{BaseUrl}}/lotecobv/{{id}}", col.Folders[2].HttpRequests[0].Url);

        Assert.Equal("Utilizado para revisar cobranças específicas dentro de um lote de cobranças com vencimento.", col.Folders[2].Requests[1].Name);
        Assert.Equal("PATCH", col.Folders[2].HttpRequests[1].HttpMethod);
        Assert.Equal("{{BaseUrl}}/lotecobv/{{id}}", col.Folders[2].HttpRequests[1].Url);

        Assert.Equal("Consultar um lote específico de cobranças com vencimento.", col.Folders[2].Requests[2].Name);
        Assert.Equal("GET", col.Folders[2].HttpRequests[2].HttpMethod);
        Assert.Equal("{{BaseUrl}}/lotecobv/{{id}}", col.Folders[2].HttpRequests[2].Url);

        Assert.Equal("Consultar lotes de cobranças com vencimento.", col.Folders[2].Requests[3].Name);
        Assert.Equal("GET", col.Folders[2].HttpRequests[3].HttpMethod);
        Assert.Contains("{{BaseUrl}}/lotecobv?inicio=", col.Folders[2].HttpRequests[3].Url);
        //------------------------
        Assert.Equal("PayloadLocation", col.Folders[3].Name);

        Assert.Equal("Criar location do payload.", col.Folders[3].Requests[0].Name);
        Assert.Equal("POST", col.Folders[3].HttpRequests[0].HttpMethod);
        Assert.Equal("{{BaseUrl}}/loc", col.Folders[3].HttpRequests[0].Url);

        Assert.Equal("Consultar locations cadastradas.", col.Folders[3].Requests[1].Name);
        Assert.Equal("GET", col.Folders[3].HttpRequests[1].HttpMethod);
        Assert.Contains("{{BaseUrl}}/loc?inicio=", col.Folders[3].HttpRequests[1].Url);

        Assert.Equal("Recuperar location do payload.", col.Folders[3].Requests[2].Name);
        Assert.Equal("GET", col.Folders[3].HttpRequests[2].HttpMethod);
        Assert.Equal("{{BaseUrl}}/loc/{{id}}", col.Folders[3].HttpRequests[2].Url);

        Assert.Equal("Desvincular uma cobrança de uma location.", col.Folders[3].Requests[3].Name);
        Assert.Equal("DELETE", col.Folders[3].HttpRequests[3].HttpMethod);
        Assert.Equal("{{BaseUrl}}/loc/{{id}}/txid", col.Folders[3].HttpRequests[3].Url);
        //------------------------
        Assert.Equal("Pix", col.Folders[4].Name);

        Assert.Equal("Consultar Pix.", col.Folders[4].Requests[0].Name);
        Assert.Equal("GET", col.Folders[4].HttpRequests[0].HttpMethod);
        Assert.Equal("{{BaseUrl}}/pix/{{e2eid}}", col.Folders[4].HttpRequests[0].Url);

        Assert.Equal("Consultar Pix recebidos.", col.Folders[4].Requests[1].Name);
        Assert.Equal("GET", col.Folders[4].HttpRequests[1].HttpMethod);
        Assert.Contains("{{BaseUrl}}/pix?inicio=", col.Folders[4].HttpRequests[1].Url);

        Assert.Equal("Solicitar devolução.", col.Folders[4].Requests[2].Name);
        Assert.Equal("PUT", col.Folders[4].HttpRequests[2].HttpMethod);
        Assert.Equal("{{BaseUrl}}/pix/{{e2eid}}/devolucao/{{id}}", col.Folders[4].HttpRequests[2].Url);

        Assert.Equal("Consultar devolução.", col.Folders[4].Requests[3].Name);
        Assert.Equal("GET", col.Folders[4].HttpRequests[3].HttpMethod);
        Assert.Equal("{{BaseUrl}}/pix/{{e2eid}}/devolucao/{{id}}", col.Folders[4].HttpRequests[3].Url);
        //------------------------
        Assert.Equal("CobPayload", col.Folders[5].Name);

        Assert.Equal("Recuperar o payload JSON que representa a cobrança imediata.", col.Folders[5].Requests[0].Name);
        Assert.Equal("GET", col.Folders[5].HttpRequests[0].HttpMethod);
        Assert.Equal("{{BaseUrl}}/{{pixUrlAccessToken}}", col.Folders[5].HttpRequests[0].Url);

        Assert.Equal("Recuperar o payload JSON que representa a cobrança com vencimento.", col.Folders[5].Requests[1].Name);
        Assert.Equal("GET", col.Folders[5].HttpRequests[1].HttpMethod);
        Assert.Equal("{{BaseUrl}}/cobv/{{pixUrlAccessToken}}", col.Folders[5].HttpRequests[1].Url);
        //------------------------
        Assert.Equal("Webhook", col.Folders[6].Name);

        Assert.Equal("Configurar o Webhook Pix.", col.Folders[6].Requests[0].Name);
        Assert.Equal("PUT", col.Folders[6].HttpRequests[0].HttpMethod);
        Assert.Equal("{{BaseUrl}}/webhook/{{chave}}", col.Folders[6].HttpRequests[0].Url);

        Assert.Equal("Exibir informações acerca do Webhook Pix.", col.Folders[6].Requests[1].Name);
        Assert.Equal("GET", col.Folders[6].HttpRequests[1].HttpMethod);
        Assert.Equal("{{BaseUrl}}/webhook/{{chave}}", col.Folders[6].HttpRequests[1].Url);

        Assert.Equal("Cancelar o webhook Pix.", col.Folders[6].Requests[2].Name);
        Assert.Equal("DELETE", col.Folders[6].HttpRequests[2].HttpMethod);
        Assert.Equal("{{BaseUrl}}/webhook/{{chave}}", col.Folders[6].HttpRequests[2].Url);

        Assert.Equal("Consultar webhooks cadastrados.", col.Folders[6].Requests[3].Name);
        Assert.Equal("GET", col.Folders[6].HttpRequests[3].HttpMethod);
        Assert.Contains("{{BaseUrl}}/webhook?inicio=", col.Folders[6].HttpRequests[3].Url);
        //------------------------

        #endregion
    }

    [Fact]
    public static void Should_read_request_with_raw_json_body_correctly()
    {
        // GIVEN
        string fileContent = GetTestFile("openapi_pix.yaml");

        // WHEN AND THEN
        Assert.True(TryImportOpenApi(fileContent, out var col));

        // THEN
        Assert.NotNull(col);

        #region REQUEST WITH RAW JSON BODY

        var req = col.Folders[1].HttpRequests[0];

        Assert.Equal("Criar cobrança com vencimento.", req.Name);
        Assert.Equal("PUT", req.HttpMethod);
        Assert.Equal("{{BaseUrl}}/cobv/{{txid}}", req.Url);
        Assert.NotNull(req.Body);
        Assert.Equal(PororocaHttpRequestBodyMode.Raw, req.Body.Mode);
        Assert.Equal("application/json", req.Body.ContentType);
        Assert.Equal(
            "{\"calendario\":{\"dataDeVencimento\":\"2020-12-31\",\"validadeAposVencimento\":\"30\"},\"loc\":{\"id\":\"789\"},\"devedor\":{\"logradouro\":\"Alameda Souza, Numero 80, Bairro Braz\",\"cidade\":\"Recife\",\"uf\":\"PE\",\"cep\":\"70011750\",\"cpf\":\"12345678909\",\"nome\":\"Francisco da Silva\"},\"valor\":{\"original\":\"123.45\",\"multa\":{\"modalidade\":\"2\",\"valorPerc\":\"15.00\"},\"juros\":{\"modalidade\":\"2\",\"valorPerc\":\"2.00\"},\"desconto\":{\"modalidade\":\"1\",\"descontoDataFixa\":[{\"data\":\"2020-11-30\",\"valorPerc\":\"30.00\"}]}},\"chave\":\"5f84a4c5-c5cb-4599-9f13-7eb4d419dacc\",\"solicitacaoPagador\":\"Cobrança dos serviços prestados.\"}",
            JsonSerializer.Serialize(JsonSerializer.Deserialize<dynamic>(req.Body.RawContent!), options: MinifyingOptions));

        #endregion
    }

    [Fact]
    public static void Should_read_request_with_query_parameters_and_empty_body_correctly()
    {
        // GIVEN
        string fileContent = GetTestFile("openapi_petstore.json");

        // WHEN AND THEN
        Assert.True(TryImportOpenApi(fileContent, out var col));

        // THEN
        Assert.NotNull(col);

        #region REQUEST WITH QUERY PARAMETERS AND EMPTY BODY

        var req = col.Folders[0].HttpRequests[5];

        Assert.Equal("Updates a pet in the store with form data", req.Name);
        Assert.Equal("POST", req.HttpMethod);
        Assert.Equal("{{BaseUrl}}/pet/{{petId}}?name=&status=", req.Url);
        Assert.Null(req.Body);

        #endregion
    }

    [Fact]
    public static void Should_read_request_with_url_encoded_body_correctly()
    {
        // GIVEN
        string fileContent = GetTestFile("openapi_imgflip.yml");

        // WHEN AND THEN
        Assert.True(TryImportOpenApi(fileContent, out var col));

        // THEN
        Assert.NotNull(col);

        #region REQUEST WITH URL ENCODED BODY

        var req = col.HttpRequests[1];

        Assert.Equal("Add a caption to an Imgflip meme template", req.Name);
        Assert.Equal("POST", req.HttpMethod);
        Assert.Equal("{{BaseUrl}}/caption_image", req.Url);
        Assert.NotNull(req.Body);
        Assert.Equal(PororocaHttpRequestBodyMode.UrlEncoded, req.Body.Mode);
        Assert.Null(req.Body.ContentType);
        Assert.NotNull(req.Body.UrlEncodedValues);
        Assert.Equal(8, req.Body.UrlEncodedValues.Count);
        Assert.Equal(new(true, "template_id", ""), req.Body.UrlEncodedValues[0]);
        Assert.Equal(new(true, "username", ""), req.Body.UrlEncodedValues[1]);
        Assert.Equal(new(true, "password", ""), req.Body.UrlEncodedValues[2]);
        Assert.Equal(new(true, "text0", ""), req.Body.UrlEncodedValues[3]);
        Assert.Equal(new(true, "text1", ""), req.Body.UrlEncodedValues[4]);
        Assert.Equal(new(true, "font", "impact"), req.Body.UrlEncodedValues[5]);
        Assert.Equal(new(true, "max_font_size", "0"), req.Body.UrlEncodedValues[6]);
        Assert.Equal(new(true, "no_watermark", "False"), req.Body.UrlEncodedValues[7]);

        #endregion
    }

    private static string GetTestFile(string fileName)
    {
        var testDataDirInfo = new DirectoryInfo(Environment.CurrentDirectory).Parent!.Parent!.Parent!;
        string jsonFileInfoPath = Path.Combine(testDataDirInfo.FullName, "TestData", "OpenAPI", fileName);
        return File.ReadAllText(jsonFileInfoPath, Encoding.UTF8);
    }
}