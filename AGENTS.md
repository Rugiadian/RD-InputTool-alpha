# RD-InputTool-alpha AI 개발 및 토큰 절약 지침 (AGENTS.md)

## 1. 아키텍처 및 모듈 분할 가이드
본 프로젝트는 WinForms 기반 C# 자동 입력 툴입니다.
대규모 단일 파일로 인한 토큰 과소비를 방지하기 위해 `MainForm`이 기능별 `partial class`로 완전 분할되어 있습니다.
**요청받은 작업과 관련된 파일만 열람하고, 불필요한 파일 전체를 읽지 마십시오.**

| 파일명 | 크기 및 역할 | 열람 기준 |
| :--- | :--- | :--- |
| **`MainForm.cs`** | 메인 생명주기 및 폼 이벤트 | 폼 로드/표시/종료/활성화 관련 생명주기 제어 시 |
| **`MainForm.Automation.cs`** | 자동 입력 루프 및 매크로 로직 | 클릭/키입력/지연시간/반복 루프/중단 처리 수정 시 |
| **`MainForm.Settings.cs`** | 옵션 설정 및 대시보드 동기화 | 설정 저장/로드, 항상 위 모드, 모드 힌트 표시 수정 시 |
| **`MainForm.Hotkey.cs`** | 단축키 감지 및 전역 키보드 훅 | 단축키 등록/변경/초기화/감지 처리 수정 시 |
| **`MainForm.Theme.cs`** | UI 테마 엔진 및 색상 정의 | 색상/스타일/테마 팔레트 수정 시 |
| **`MainForm.Designer.cs`** | UI 컨트롤 생성 및 배치 전용 | 레이아웃/컨트롤 추가 또는 UI 배치 변경 시에만 참조 |
| **`AppSettings.cs`** | JSON 직렬화 설정 모델 | 영구 저장될 설정 속성 정의 수정 시 |
| **`InputSimulator.cs`** | 저수준 Win32 마우스/키보드 전송 | SendInput, 키보드/마우스 API 동작 수정 시 |
| **`NativeMethods.cs`** | Win32 P/Invoke API 선언부 | Windows API 추가 선언 필요 시 |
| **`EnergyBar.cs`** | 커스텀 프로그레스바 컨트롤 | 게이지 그래픽/애니메이션 수정 시 |
| **`CoordinatePickerForm.cs`** | 좌표 선택 오버레이 화면 | 화면 마우스 좌표 픽커 수정 시 |
| **`HotkeyCaptureDialog.cs`** | 단축키 입력 감지 팝업 | 단축키 입력 모달 수정 시 |

---

## 2. AI 토큰 절약 필수 준수 규칙 (Token Saving Rules)
1. **필요한 파일만 최소 열람**:
   - 로직 수정 시 1,000줄이 넘는 `MainForm.Designer.cs`를 절대 통째로 읽지 마십시오.
   - 핫키 문제는 `MainForm.Hotkey.cs`, 매크로 동작은 `MainForm.Automation.cs`만 타겟팅하십시오.
2. **범위 지정 조회(StartLine/EndLine) 활용**:
   - 파일의 특정 메서드만 확인할 때는 파일 전체를 읽지 말고 해당 라인 범위만 조회하십시오.
3. **바이너리/빌드 폴더 접근 금지**:
   - `bin/`, `obj/`, `Publish/` 폴더는 컴파일 산출물이므로 인덱싱, 검색, 텍스트 열람 대상에서 완전히 배제하십시오.
4. **빌드 명령어**:
   - 프로젝트 빌드는 다음 명령어로 확인 가능합니다:
     `& "$env:LOCALAPPDATA\Microsoft\dotnet\dotnet.exe" build RD_Tools.csproj`
