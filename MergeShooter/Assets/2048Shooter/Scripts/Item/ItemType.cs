using System;

public enum ItemType
{
    None,
	selectDeleteBrick,      // 선택한 블록 1개 제거
    selectUpgradeBrick,     // 선택한 블록 한단계 업그레이드
    underNumberDeleteBrick, // 특정 숫자 이하 블록 모두 제거
    lineDelete,             // 한 라인 모두 제거
    selectBrickChangeBoomb, // 선택한 블록 폭탄 블록으로
    stopTime                // 시간 정지
}
