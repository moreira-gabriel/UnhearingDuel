using UnityEngine;

/// <summary>
/// - Interface que detecta o click do mouse em algum objeto
/// - Esse c�digo deve ir no objeto que ir� sofrer o click
/// - Assim que o mouse clicar o objeto o script que tem a interface como heran�a vai disparar o m�todo da interface e executar o comando que estiver dentro dele
/// </summary>

public interface IClicker
{
    void Click();
}
