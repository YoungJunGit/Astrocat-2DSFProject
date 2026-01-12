// 이 Ink 연동 예제 씬 버전에는 다음과 같은 변경 사항이 있습니다:
// 1. Dialogue System의 외부 함수들을 사용하며, 파일 하단에 fallback 함수가 정의되어 있습니다.
// 2. 화자가 모호한 경우를 구분하기 위해 Actor=PlayerSpeaker 태그를 사용합니다.
// 3. wagerAmount 변수를 정의하고, 스토리 진행 중 20000으로 설정합니다.
//    Dialogue Editor에서 이 변수에 Watch를 걸어 값 변화를 확인할 수 있습니다.


EXTERNAL ShowAlert(x)                 // ShowAlert("메시지")
EXTERNAL CurrentQuestState(x)         // CurrentQuestState("퀘스트")
EXTERNAL CurrentQuestEntryState(x,y)  // CurrentQuestEntryState("퀘스트", 엔트리 번호)
EXTERNAL SetQuestState(x,y)           // SetQuestState("퀘스트", "inactive|active|success|failure")
EXTERNAL SetQuestEntryState(x,y,z)    // SetQuestEntryState("퀘스트", 엔트리 번호, "inactive|active|success|failure")

VAR wagerAmount = 0

- 나는 몽슈 푸그를 바라보았다.  # Actor=Player
*   ... 그리고 더 이상 참을 수가 없었다.
    '이번 여행의 목적이 무엇입니까, 몽슈?' # Actor=Player
    '내기지.' 그가 대답했다. { SetQuestState("The Wager", "active") } { ShowAlert("퀘스트: The Wager") }
    * *     '내기라고요!'[] 나는 되물었다.
            그는 고개를 끄덕였다.
            * * *   '하지만 그건 분명 어리석은 짓 아닙니까!'
            * * *   '그렇다면 아주 중대한 문제군요!'
            - - -   그는 다시 한번 고개를 끄덕였다.
            * * *   '하지만 우리가 이길 수 있을까요?'
                    '그건 이제부터 알아내야겠지.' 그가 대답했다.
            * * *   '설마 소액 내기는 아니겠죠?'
                    '이만 파운드.' 그는 담담하게 말했다.
					~ wagerAmount = 20000
            * * *   나는 더 이상 아무것도 묻지 않았고[.], 마지막으로 정중하게 헛기침을 한 뒤, 그는 더 이상 아무 말도 하지 않았다. <>
    * *     '아[.]…' 나는 무슨 생각을 해야 할지 모른 채 대답했다.
    - -     그 후로는, <>
*   ... 하지만 나는 아무 말도 하지 않았고[] <>
- 우리는 하루를 침묵 속에서 보냈다.
- -> END


// Fallback 함수들

== function ShowAlert(x) ==
~ return 1

== function CurrentQuestState(x) ==
~ return "inactive"

== function CurrentQuestEntryState(x,y) ==
~ return "inactive"

== function SetQuestState(x,y) ==
~ return 1

== function SetQuestEntryState(x,y,z) ==
~ return 1
