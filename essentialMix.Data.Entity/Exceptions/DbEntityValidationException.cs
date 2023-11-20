using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using JetBrains.Annotations;

namespace essentialMix.Data.Entity.Exceptions;

[Serializable]
public class DbEntityValidationException : DataException
{
	private ICollection<ValidationResult> _entityValidationResults;

	/// <summary>
	/// Initializes a new instance of DbEntityValidationException.
	/// </summary>
	public DbEntityValidationException()
		: this("Validation failed")
	{
	}

	/// <summary>
	/// Initializes a new instance of DbEntityValidationException.
	/// </summary>
	/// <param name="message"> The exception message. </param>
	public DbEntityValidationException(string message)
		: this(message, Enumerable.Empty<ValidationResult>())
	{
	}

	/// <summary>
	/// Initializes a new instance of DbEntityValidationException.
	/// </summary>
	/// <param name="message"> The exception message. </param>
	/// <param name="entityValidationResults"> Validation results. </param>
	public DbEntityValidationException(string message, [NotNull] IEnumerable<ValidationResult> entityValidationResults)
		: base(message)
	{
		InitializeValidationResults(entityValidationResults);
	}

	/// <summary>
	/// Initializes a new instance of DbEntityValidationException.
	/// </summary>
	/// <param name="message"> The exception message. </param>
	/// <param name="innerException"> The inner exception. </param>
	public DbEntityValidationException(string message, Exception innerException)
		: this(message, Enumerable.Empty<ValidationResult>(), innerException)
	{
	}

	/// <summary>
	/// Initializes a new instance of DbEntityValidationException.
	/// </summary>
	/// <param name="message"> The exception message. </param>
	/// <param name="entityValidationResults"> Validation results. </param>
	/// <param name="innerException"> The inner exception. </param>
	public DbEntityValidationException(string message, [NotNull] IEnumerable<ValidationResult> entityValidationResults, Exception innerException)
		: base(message, innerException)
	{
		InitializeValidationResults(entityValidationResults);
	}

	/// <summary>Validation results.</summary>
	public IEnumerable<ValidationResult> EntityValidationErrors => _entityValidationResults;


	private void InitializeValidationResults([NotNull] IEnumerable<ValidationResult> entityValidationResults)
	{
		_entityValidationResults = entityValidationResults as ICollection<ValidationResult> ?? entityValidationResults.ToList();
	}
}