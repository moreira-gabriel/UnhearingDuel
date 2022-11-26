using UnityEngine;

/// <summary>
/// - Interface que detecta o click do mouse em algum objeto
/// - Esse código deve ir no objeto que irá sofrer o click
/// - Assim que o mouse clicar o objeto o script que tem a interface como herança vai disparar o método da interface e executar o comando que estiver dentro dele
/// </summary>

public interface IClicker
{
    void Click();
}
