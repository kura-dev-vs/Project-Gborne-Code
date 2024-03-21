using Cinemachine;
using UnityEngine;

public class CinemachineUserInputZoom : CinemachineExtension
{
    // Input Managerの入力名
    [SerializeField] private string _inputName = "Mouse ScrollWheel";

    // 入力値に掛ける値
    [SerializeField] private float _inputScale = 100;

    // FOVの最小・最大
    [SerializeField, Range(1, 179)] private float _minFOV = 10;
    [SerializeField, Range(1, 179)] private float _maxFOV = 90;

    // ユーザー入力を必要とする
    public override bool RequiresUserInput => true;

    private float _scrollDelta;
    private float _adjustFOV;

    private void Update()
    {
        // スクロール量を加算
        _scrollDelta += Input.GetAxis(_inputName);
    }

    // 各ステージ毎に実行されるコールバック
    protected override void PostPipelineStageCallback(
        CinemachineVirtualCameraBase vcam,
        CinemachineCore.Stage stage,
        ref CameraState state,
        float deltaTime
    )
    {
        // Aimの直後だけ処理を実施
        if (stage != CinemachineCore.Stage.Aim)
            return;

        var lens = state.Lens;

        if (!Mathf.Approximately(_scrollDelta, 0))
        {
            // FOVの補正量を計算
            _adjustFOV = Mathf.Clamp(
                _adjustFOV - _scrollDelta * _inputScale,
                _minFOV - lens.FieldOfView,
                _maxFOV - lens.FieldOfView
            );

            _scrollDelta = 0;
        }

        // stateの内容は毎回リセットされるので、
        // 毎回補正する必要がある
        lens.FieldOfView += _adjustFOV;

        state.Lens = lens;
    }
}