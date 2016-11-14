#pragma once

#include <memory>
#include <wrl/client.h>


// Unity interface and ID3D11Device getters
struct IUnityInterfaces;
IUnityInterfaces* GetUnity();

struct ID3D11Device;
Microsoft::WRL::ComPtr<ID3D11Device> GetDevice();


// Manager getter
class MonitorManager;
const std::unique_ptr<MonitorManager>& GetMonitorManager();


template <class T>
auto MakeUniqueWithReleaser(T* ptr)
{
    const auto deleter = [](T* ptr) 
    { 
        if (ptr != nullptr) ptr->Release();
    };
    return std::unique_ptr<T, decltype(deleter)>(ptr, deleter);
}


// Message is pooled and fetch from Unity.
enum class Message
{
    None = -1,
    Reinitialized = 0,
	TextureSizeChanged = 1,
};

void SendMessageToUnity(Message message);