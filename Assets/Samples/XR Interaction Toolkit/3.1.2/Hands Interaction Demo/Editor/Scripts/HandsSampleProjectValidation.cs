using System;
using System.Collections.Generic;
using System.Linq;
using Unity.XR.CoreUtils.Editor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEditor.PackageManager.UI;
using UnityEditor.XR.Interaction.Toolkit.ProjectValidation;
using UnityEngine;

namespace UnityEditor.XR.Interaction.Toolkit.Samples.Hands.Editor
{
    /// <summary>
    ///     Unity Editor class which registers Project Validation rules for the Hands Interaction Demo sample,
    ///     checking that other required samples and packages are installed.
    /// </summary>
    internal static class HandsSampleProjectValidation
    {
        private const string k_SampleDisplayName = "Hands Interaction Demo";
        private const string k_Category = "XR Interaction Toolkit";
        private const string k_StarterAssetsSampleName = "Starter Assets";
        private const string k_HandVisualizerSampleName = "HandVisualizer";
        private const string k_ProjectValidationSettingsPath = "Project/XR Plug-in Management/Project Validation";
        private const string k_HandsPackageDisplayName = "XR Hands";
        private const string k_HandsPackageName = "com.unity.xr.hands";
        private const string k_XRIPackageName = "com.unity.xr.interaction.toolkit";
        private const string k_ShaderGraphPackageName = "com.unity.shadergraph";
        private static readonly PackageVersion s_MinimumHandsPackageVersion = new("1.2.1");
        private static readonly PackageVersion s_RecommendedHandsPackageVersion = new("1.3.0");

        private static readonly BuildTargetGroup[] s_BuildTargetGroups =
            ((BuildTargetGroup[])Enum.GetValues(typeof(BuildTargetGroup))).Distinct().ToArray();

        private static readonly List<BuildValidationRule> s_BuildValidationRules = new()
        {
            new()
            {
                IsRuleEnabled = () => s_HandsPackageAddRequest == null || s_HandsPackageAddRequest.IsCompleted,
                Message =
                    $"[{k_SampleDisplayName}] XR Hands ({k_HandsPackageName}) package must be installed or updated to use this sample.",
                Category = k_Category,
                CheckPredicate = () =>
                    PackageVersionUtility.GetPackageVersion(k_HandsPackageName) >= s_MinimumHandsPackageVersion,
                FixIt = () =>
                {
                    if (s_HandsPackageAddRequest == null || s_HandsPackageAddRequest.IsCompleted)
                        InstallOrUpdateHands();
                },
                FixItAutomatic = true,
                Error = true
            },
            new()
            {
                IsRuleEnabled = () => s_HandsPackageAddRequest == null || s_HandsPackageAddRequest.IsCompleted,
                Message =
                    $"[{k_SampleDisplayName}] XR Hands ({k_HandsPackageName}) package must be at version {s_RecommendedHandsPackageVersion} or higher to use the latest sample features.",
                Category = k_Category,
                CheckPredicate = () =>
                    PackageVersionUtility.GetPackageVersion(k_HandsPackageName) >= s_RecommendedHandsPackageVersion,
                FixIt = () =>
                {
                    if (s_HandsPackageAddRequest == null || s_HandsPackageAddRequest.IsCompleted)
                        InstallOrUpdateHands();
                },
                FixItAutomatic = true,
                Error = false
            },
            new()
            {
                IsRuleEnabled = () =>
                    PackageVersionUtility.GetPackageVersion(k_HandsPackageName) >= s_MinimumHandsPackageVersion,
                Message =
                    $"[{k_SampleDisplayName}] {k_HandVisualizerSampleName} sample from XR Hands ({k_HandsPackageName}) package must be imported or updated to use this sample.",
                Category = k_Category,
                CheckPredicate = () =>
                    ProjectValidationUtility.SampleImportMeetsMinimumVersion(k_HandsPackageDisplayName,
                        k_HandVisualizerSampleName, PackageVersionUtility.GetPackageVersion(k_HandsPackageName)),
                FixIt = () =>
                {
                    if (TryFindSample(k_HandsPackageName, string.Empty, k_HandVisualizerSampleName, out var sample))
                        sample.Import(Sample.ImportOptions.OverridePreviousImports);
                },
                FixItAutomatic = true,
                Error = !ProjectValidationUtility.HasSampleImported(k_HandsPackageDisplayName,
                    k_HandVisualizerSampleName)
            },
            new()
            {
                Message =
                    $"[{k_SampleDisplayName}] {k_StarterAssetsSampleName} sample from XR Interaction Toolkit ({k_XRIPackageName}) package must be imported or updated to use this sample. {GetImportSampleVersionMessage(k_Category, k_StarterAssetsSampleName, ProjectValidationUtility.minimumXRIStarterAssetsSampleVersion)}",
                Category = k_Category,
                CheckPredicate = () => ProjectValidationUtility.SampleImportMeetsMinimumVersion(k_Category,
                    k_StarterAssetsSampleName, ProjectValidationUtility.minimumXRIStarterAssetsSampleVersion),
                FixIt = () =>
                {
                    if (TryFindSample(k_XRIPackageName, string.Empty, k_StarterAssetsSampleName, out var sample))
                        sample.Import(Sample.ImportOptions.OverridePreviousImports);
                },
                FixItAutomatic = true,
                Error = !ProjectValidationUtility.HasSampleImported(k_Category, k_StarterAssetsSampleName)
            },
            new()
            {
                IsRuleEnabled = () =>
                    s_ShaderGraphPackageAddRequest == null || s_ShaderGraphPackageAddRequest.IsCompleted,
                Message =
                    $"[{k_SampleDisplayName}] Shader Graph ({k_ShaderGraphPackageName}) package must be installed for materials used in this sample.",
                Category = k_Category,
                CheckPredicate = () => PackageVersionUtility.IsPackageInstalled(k_ShaderGraphPackageName),
                FixIt = () =>
                {
                    s_ShaderGraphPackageAddRequest = Client.Add(k_ShaderGraphPackageName);
                    if (s_ShaderGraphPackageAddRequest.Error != null)
                        Debug.LogError(
                            $"Package installation error: {s_ShaderGraphPackageAddRequest.Error}: {s_ShaderGraphPackageAddRequest.Error.message}");
                },
                FixItAutomatic = true,
                Error = false
            }
        };

        private static AddRequest s_HandsPackageAddRequest;
        private static AddRequest s_ShaderGraphPackageAddRequest;

        [InitializeOnLoadMethod]
        private static void RegisterProjectValidationRules()
        {
            foreach (var buildTargetGroup in s_BuildTargetGroups)
                BuildValidator.AddRules(buildTargetGroup, s_BuildValidationRules);

            // Delay evaluating conditions for issues to give time for Package Manager and UPM cache to fully initialize.
            EditorApplication.delayCall += ShowWindowIfIssuesExist;
        }

        private static void ShowWindowIfIssuesExist()
        {
            foreach (var validation in s_BuildValidationRules)
                if (validation.CheckPredicate == null || !validation.CheckPredicate.Invoke())
                {
                    ShowWindow();
                    return;
                }
        }

        internal static void ShowWindow()
        {
            // Delay opening the window since sometimes other settings in the player settings provider redirect to the
            // project validation window causing serialized objects to be nullified.
            EditorApplication.delayCall += () =>
            {
                SettingsService.OpenProjectSettings(k_ProjectValidationSettingsPath);
            };
        }

        private static bool TryFindSample(string packageName, string packageVersion, string sampleDisplayName,
            out Sample sample)
        {
            sample = default;

            if (!PackageVersionUtility.IsPackageInstalled(packageName))
                return false;

            IEnumerable<Sample> packageSamples;
            try
            {
                packageSamples = Sample.FindByPackage(packageName, packageVersion);
            }
            catch (Exception e)
            {
                Debug.LogError(
                    $"Couldn't find samples of the {ToString(packageName, packageVersion)} package; aborting project validation rule. Exception: {e}");
                return false;
            }

            if (packageSamples == null)
            {
                Debug.LogWarning(
                    $"Couldn't find samples of the {ToString(packageName, packageVersion)} package; aborting project validation rule.");
                return false;
            }

            foreach (var packageSample in packageSamples)
                if (packageSample.displayName == sampleDisplayName)
                {
                    sample = packageSample;
                    return true;
                }

            Debug.LogWarning(
                $"Couldn't find {sampleDisplayName} sample in the {ToString(packageName, packageVersion)} package; aborting project validation rule.");
            return false;
        }

        private static string ToString(string packageName, string packageVersion)
        {
            return string.IsNullOrEmpty(packageVersion) ? packageName : $"{packageName}@{packageVersion}";
        }

        private static void InstallOrUpdateHands()
        {
            // Set a 3-second timeout for request to avoid editor lockup
            var currentTime = DateTime.Now;
            var endTime = currentTime + TimeSpan.FromSeconds(3);

            var request = Client.Search(k_HandsPackageName);
            if (request.Status == StatusCode.InProgress)
            {
                Debug.Log($"Searching for ({k_HandsPackageName}) in Unity Package Registry.");
                while (request.Status == StatusCode.InProgress && currentTime < endTime)
                    currentTime = DateTime.Now;
            }

            var addRequest = k_HandsPackageName;
            if (request.Status == StatusCode.Success && request.Result.Length > 0)
            {
                var versions = request.Result[0].versions;
#if UNITY_2022_2_OR_NEWER
                var recommendedVersion = new PackageVersion(versions.recommended);
#else
                var recommendedVersion = new PackageVersion(versions.verified);
#endif
                var latestCompatible = new PackageVersion(versions.latestCompatible);
                if (recommendedVersion < s_RecommendedHandsPackageVersion &&
                    s_RecommendedHandsPackageVersion <= latestCompatible)
                    addRequest = $"{k_HandsPackageName}@{s_RecommendedHandsPackageVersion}";
            }

            s_HandsPackageAddRequest = Client.Add(addRequest);
            if (s_HandsPackageAddRequest.Error != null)
                Debug.LogError(
                    $"Package installation error: {s_HandsPackageAddRequest.Error}: {s_HandsPackageAddRequest.Error.message}");
        }

        private static string GetImportSampleVersionMessage(string packageFolderName, string sampleDisplayName,
            PackageVersion version)
        {
            if (ProjectValidationUtility.SampleImportMeetsMinimumVersion(packageFolderName, sampleDisplayName,
                    version) || !ProjectValidationUtility.HasSampleImported(packageFolderName, sampleDisplayName))
                return string.Empty;

            return $"An older version of {sampleDisplayName} has been found. This may cause errors.";
        }
    }
}