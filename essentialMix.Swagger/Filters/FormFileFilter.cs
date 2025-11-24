using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using essentialMix.Extensions;
using essentialMix.Web;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace essentialMix.Swagger.Filters;

public class FormFileFilter : IOperationFilter
{
	/// <inheritdoc />
	public void Apply([NotNull] OpenApiOperation operation, [NotNull] OperationFilterContext context)
	{
		if (operation.Deprecated
			|| !string.Equals(context.ApiDescription.HttpMethod, HttpMethod.Post.Method, StringComparison.OrdinalIgnoreCase)
			&& !string.Equals(context.ApiDescription.HttpMethod, HttpMethod.Put.Method, StringComparison.OrdinalIgnoreCase))
		{
			return;
		}

		// Check if any of the parameters' types or their nested properties / fields are supported
		if (!Enumerate(context.ApiDescription.ActionDescriptor).Any(e => IsSupported(e.ParameterType))) return;

		OpenApiMediaType uploadFileMediaType = operation.RequestBody.Content.GetOrAdd(MediaTypeNames.Multipart.FormData, _ => new OpenApiMediaType
		{
			Schema = new OpenApiSchema
			{
				Type = JsonSchemaType.Object,
				Properties = new Dictionary<string, IOpenApiSchema>(StringComparer.OrdinalIgnoreCase),
				Required = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
			}
		});

		IDictionary<string, IOpenApiSchema> schemaProperties = uploadFileMediaType.Schema.Properties;
		ISet<string> schemaRequired = uploadFileMediaType.Schema.Required;
		ISchemaGenerator generator = context.SchemaGenerator;
		SchemaRepository repository = context.SchemaRepository;

		foreach (ParameterDescriptor parameter in Enumerate(context.ApiDescription.ActionDescriptor))
		{
			OpenApiSchema schema = generator.GenerateSchema(parameter.ParameterType, repository) as OpenApiSchema;
			if (schema == null) continue;

			if (IsSupported(parameter.ParameterType))
			{
				schema.Type = JsonSchemaType.String;
                schema.Format = "binary";
			}
			schemaProperties.Add(parameter.Name, schema);

			if (parameter.ParameterType.IsPrimitive && !parameter.ParameterType.IsNullable()
				|| !parameter.ParameterType.IsInterface && !parameter.ParameterType.IsClass || parameter.ParameterType.HasAttribute<RequiredAttribute>())
			{
				schemaRequired.Add(parameter.Name);
			}
		}

		[ItemNotNull]
		static IEnumerable<ParameterDescriptor> Enumerate(ActionDescriptor descriptor)
		{
			foreach (ParameterDescriptor parameter in descriptor.Parameters.Where(p => p.BindingInfo?.BindingSource != null && p.BindingInfo.BindingSource.IsFromRequest && !p.BindingInfo.BindingSource.Id.IsSame("Path")))
			{
				yield return parameter;
			}
		}
	}

	[MethodImpl(MethodImplOptions.ForwardRef | MethodImplOptions.AggressiveInlining)]
	private static bool IsSupported(Type type)
	{
		return type != null && (type.IsAssignableFrom(typeof(IFormFile)) || type.IsAssignableFrom(typeof(IFormFileCollection)));
	}
}