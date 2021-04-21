using System;
using System.Collections.Generic;
using EzPlatform.EzOnboarding.Core.ConfigModel;
using YamlDotNet.RepresentationModel;

namespace EzPlatform.EzOnboarding.Core.Serialization
{
    public static class ConfigStageParser
    {
        public static void HandleStageMapping(YamlSequenceNode yamlSequenceNode, List<ConfigStage> stages, ConfigApplication application)
        {
            foreach (var child in yamlSequenceNode.Children)
            {
                YamlParser.ThrowIfNotYamlMapping(child);
                var stage = new ConfigStage();
                HandleStageNameMapping((YamlMappingNode)child, stage, application);
                stages.Add(stage);
            }
        }

        private static void HandleStageNameMapping(YamlMappingNode yamlMappingNode, ConfigStage stage, ConfigApplication application)
        {
            foreach (var child in yamlMappingNode!.Children)
            {
                var key = YamlParser.GetScalarValue(child.Key);

                switch (key)
                {
                    case "stage":
                        stage.Stage = YamlParser.GetScalarValue(key, child.Value).ToLowerInvariant();
                        break;
                    case "displayName":
                        stage.DisplayName = YamlParser.GetScalarValue(key, child.Value).ToLowerInvariant();
                        break;
                    case "dependsOn":
                        stage.DependsOn = YamlParser.GetScalarValue(key, child.Value).ToLowerInvariant();
                        break;
                    case "tasks":
                        YamlParser.ThrowIfNotYamlSequence(key, child.Value);

                        HandleStageTasks((child.Value as YamlSequenceNode)!, stage.Tasks);
                        break;
                    default:
                        throw new EzobYamlException(child.Key.Start, string.Format(CoreStrings.UnrecognizedKey, key));
                }
            }
        }

        private static void HandleStageTasks(YamlSequenceNode yamlSequenceNode, List<ConfigTask> tasks)
        {
            foreach (var child in yamlSequenceNode.Children)
            {
                YamlParser.ThrowIfNotYamlMapping(child);
                var task = new ConfigTask();
                HandleStageTaskNameMapping((YamlMappingNode)child, task);
                tasks.Add(task);
            }
        }

        private static void HandleStageTaskNameMapping(YamlMappingNode yamlMappingNode, ConfigTask task)
        {
            foreach (var child in yamlMappingNode.Children)
            {
                var key = YamlParser.GetScalarValue(child.Key);

                switch (key)
                {
                    case "task":
                        task.Task = Enum.Parse<ConfigTaskType>(YamlParser.GetScalarValue(key, child.Value));
                        break;
                    case "displayName":
                        task.DisplayName = YamlParser.GetScalarValue(key, child.Value);
                        break;
                    case "inputs":
                        HandleInputsMapping((child.Value as YamlMappingNode)!, task);
                        break;
                    default:
                        throw new EzobYamlException(child.Key.Start, string.Format(CoreStrings.UnrecognizedKey, key));
                }
            }
        }

        private static void HandleInputsMapping(YamlMappingNode yamlMappingNode, ConfigTask task)
        {
            switch (task.Task)
            {
                case ConfigTaskType.CreateTicket:
                    task.Inputs = CreateTicketInputsParser.HandleInputsMapping(yamlMappingNode);
                    break;

                case ConfigTaskType.InstallSoftware:
                    task.Inputs = InstallSoftwareInputsParser.HandleInputsMapping(yamlMappingNode);
                    break;

                case ConfigTaskType.SourceCodeCheckout:
                    task.Inputs = SourceCodeCheckoutInputsParser.HandleInputsMapping(yamlMappingNode);
                    break;

                default:
                    break;
            }
        }
    }
}
