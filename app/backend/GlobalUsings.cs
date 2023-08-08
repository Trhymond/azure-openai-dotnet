global using Microsoft.Azure.Functions.Worker;
global using Microsoft.Azure.Functions.Worker.Http;
global using System.Net;
global using System.Diagnostics;
global using System.ComponentModel;
global using System.Runtime.CompilerServices;
global using Microsoft.Extensions.Logging;
global using System.Text;
global using System.Text.Json;
global using Microsoft.Extensions.Caching.Distributed;
global using Azure.AI.FormRecognizer.DocumentAnalysis;
global using Azure.AI.OpenAI;
global using Azure.Identity;
global using Azure.Search.Documents;
global using Azure.Search.Documents.Models;
global using Azure.Storage.Blobs;
global using Microsoft.ML;
global using Microsoft.ML.Transforms.Text;
global using Microsoft.SemanticKernel;
global using Microsoft.SemanticKernel.AI;
global using Microsoft.SemanticKernel.AI.Embeddings;
global using Microsoft.SemanticKernel.AI.TextCompletion;
global using Microsoft.SemanticKernel.Memory;
global using Microsoft.SemanticKernel.Orchestration;
global using Microsoft.SemanticKernel.SkillDefinition;
global using Microsoft.SemanticKernel.Planning;
global using Microsoft.SemanticKernel.Planning.Sequential;
global using Microsoft.Azure.Functions.Extensions.DependencyInjection;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Configuration;
global using Rhymond.OpenAI;
global using Rhymond.OpenAI.Plugins;
global using Rhymond.OpenAI.Models;
global using Rhymond.OpenAI.Extensions;
global using Rhymond.OpenAI.Services;
