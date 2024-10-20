using System;

namespace SPIC.Configs;

public interface IConfigProvider {
    Type ConfigType { get; }
    object Config { set; }
    void OnLoaded(bool created) { }
    ProviderDefinition ProviderDefinition { get; }
}

public interface IConfigProvider<TConfig> : IConfigProvider where TConfig : new() {
    new TConfig Config { set; }
    Type IConfigProvider.ConfigType => typeof(TConfig);
    object IConfigProvider.Config { set => Config = (TConfig)value; }
}

public interface IClientConfigProvider {
    Type ConfigType { get; }
    object ClientConfig { set; }
    void OnLoadedClient(bool created) { }
    ProviderDefinition ProviderDefinition { get; }
}

public interface IClientConfigProvider<TConfig> : IClientConfigProvider where TConfig : new() {
    new TConfig ClientConfig { set; }
    Type IClientConfigProvider.ConfigType => typeof(TConfig);
    object IClientConfigProvider.ClientConfig { set => ClientConfig = (TConfig)value; }
}
