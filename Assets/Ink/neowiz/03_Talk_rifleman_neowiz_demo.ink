VAR DEBUG = false
VAR trust_rifleman = 0
VAR first_meet_check = false

저 앞에 소총병이 보인다. 말을 걸어볼까?

+ [좋아]
    말을 걸어봐야 겠다.
    { first_meet_check:
        -> rifleman_talk_start
    - else:
        -> rifleman_talk_first
    }

+ [아니]
    나중에 이야기하자.
    -> END


=== rifleman_talk_first ===
~ first_meet_check = true

아, 함장!
어― 잠깐만요.
이렇게 바로 오실 줄은 몰랐는데.
아 저요?
소총병이에요.
앞으로 잘 부탁드려요!

* 그녀가 손을 내밀어 악수를 청한다.

-> rifleman_talk_topic_select


=== rifleman_talk_start ===
{DEBUG:
(DEBUG) trust_rifleman = {trust_rifleman}
}

{
- trust_rifleman < 30:
    {~ 안녕하세요! 반갑습니다. | 오늘 컨디션 어때요? | 좋은 아침입니다! }
- trust_rifleman >= 30 && trust_rifleman < 50:
    {~ 안녕이에요! | 오늘은 여유 있어 보이네요. | 기분 좋아 보이세요! }
- else:
    {~ 오! 함장 왔어요? 기다리고 있었어요! | 안녕! 오늘은 제가 먼저 인사했네요? }
}

+ [소총병과 대화를 시작한다.]
    -> rifleman_talk_topic_select

+ [떠난다.]
    -> END


=== rifleman_talk_topic_select ===
무슨 일이에요?

* [이 함선 꽤나 복잡하네.]
    -> rifleman_talk_topic_1

* [제일 기억에 남는 임무는 뭐였어?]
    -> rifleman_talk_topic_2

* [특기는 뭐야?]
    -> rifleman_talk_topic_3

+ ->
    (이제 다른 사람과 대화 해보자.)
    -> END


=== rifleman_talk_topic_1 ===
맞아요. 이 함선은 꽤 커요.
-> rifleman_talk_topic_select


=== rifleman_talk_topic_2 ===
제일 기억에 남는 임무요?
흠....
아무래도 제가 제일 처음 맡았던 임무가 제일 기억에 남아요.
-> rifleman_talk_topic_select


=== rifleman_talk_topic_3 ===
제 특기요?
아하하.. 갑자기 물어보려니까 생각이 안나네요...
아..! 저는 외계 종족 언어 2급 자격증이 있어요!
그렇게 높은 등급은 아니지만..
상대방이 우호적인지 적대적인지 정도는 구별 가능해요!
-> rifleman_talk_topic_select