﻿// -------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using System.Text.RegularExpressions;
using Dicom;
using Microsoft.Health.Dicom.Core.Exceptions;
using Microsoft.Health.Dicom.Core.Extensions;

namespace Microsoft.Health.Dicom.Core.Features.Validation
{
    internal class UidValidation : ElementValidation
    {
        private static readonly Regex ValidIdentifierCharactersFormat = new Regex("^[0-9\\.]*[0-9]$", RegexOptions.Compiled);
        private const int MaxLength = 64;
        public override void Validate(DicomElement dicomElement)
        {
            base.Validate(dicomElement);

            string value = dicomElement.Get<string>();
            string name = dicomElement.Tag.GetFriendlyName();
            Validate(value, name, allowEmpty: true);
        }

        public static void Validate(string value, string name, bool allowEmpty = false)
        {
            if (string.IsNullOrEmpty(value))
            {
                if (allowEmpty)
                {
                    return;
                }

                throw new InvalidIdentifierException(value, name, DicomCoreResource.DicomIdentifierIsRequired);
            }

            // trailling spaces are allowed
            value = value.TrimEnd(' ');

            if (value.Length > MaxLength)
            {
                // UI value is validated in other cases like params for WADO, DELETE. So keeping the exception specific.
                throw new InvalidIdentifierException(value.Truncate(MaxLength), name, DicomCoreResource.DicomIdentifierExceedsMaxLength);
            }

            if (!ValidIdentifierCharactersFormat.IsMatch(value))
            {
                throw new InvalidIdentifierException(value.Truncate(MaxLength), name, DicomCoreResource.DicomIdentifierContainsInvalidCharacter);
            }
        }

    }
}