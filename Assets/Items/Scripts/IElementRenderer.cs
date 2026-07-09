using Cysharp.Threading.Tasks;

public interface IElementRenderer {
    UniTask Initialize(IElement element);
}