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
이렇게 바로 오실 줄은 몰랐어요.
아 저요?
소총병이에요. 마리나라고 불러주셔도 되고요.
이제 막 들어온 신참이에요.
아직은 많이 부족해요.
훈련도, 실전도요.
그래도.... 최대한 발목 잡지 않게 노력할게요.
앞으로 잘 부탁드려요!

* [그녀가 조심스럽게 손을 내밀어 악수를 청한다.]
- 앞으로 잘 부탁드릴게요, 함장님!

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
맞아요! 처음 봤을 때 깜짝 놀랐어요.

생각했던 것보다 훨씬 크더라고요.

복도도 많고, 방도 많고…<br>처음엔 거의 미로 같았어요.

하부 덱에서 길을 한 번 잃었는데요,<br>돌아보니까 같은 곳을 세 바퀴나 돌았더라고요.

그땐 좀 웃겼어요.<br>아, 내가 진짜 큰 함선에 올라탔구나 싶어서요.

* [적응하는 데 꽤 걸리겠네.]
    -> rifleman_topic1_response_1

* [그래도 금방 익숙해질 것 같은데.]
    -> rifleman_topic1_response_2

* [이런 함선은 처음이지?]
    -> rifleman_topic1_response_3

=== rifleman_topic1_response_1 ===
그렇죠?<br>그래서 요즘은 일부러 돌아다니면서 외우고 있어요.

길 헷갈리면 안 되니까요.<br>언젠가는 눈 감고도 다닐 수 있을 거예요!

-> rifleman_talk_topic_1_continue

=== rifleman_topic1_response_2 ===
아, 그렇게 봐주시면 다행이에요!

저도 왠지 그럴 것 같아요.<br>하루하루 조금씩 익숙해지는 느낌이거든요.

-> rifleman_talk_topic_1_continue

=== rifleman_topic1_response_3 ===
네! 이런 규모의 함선은 처음이에요.

그래서 더 신나요.<br>이 안에서 어떤 일들이 벌어질지,<br>괜히 기대하게 되고요.

-> rifleman_talk_topic_1_continue

=== rifleman_talk_topic_1_continue ===
이런 큰 함선에 올라타고 나니까요,
아, 이제 진짜 용병이 됐구나.<br>그런 실감이 나요.

아직 모르는 것도 많고,<br>실수도 하겠지만…
그래도 여기서라면<br>뭔가 멋진 일들이 시작될 것 같아요.

-> rifleman_talk_topic_select


=== rifleman_talk_topic_2 ===
제일 기억에 남는 임무요?

음…
막 대단한 작전은 아니었어요.

호전적인 외계 종족이랑 소규모 교전이 있었던 임무였는데요.

말이 안 통하는 타입이라,<br>처음부터 분위기가 좀 살벌했어요.

처음엔 괜찮았어요.<br>훈련 때 했던 대로 하면 된다고 생각했고요.

그런데 갑자기 적들의 움직임이 빨라졌고,<br>엄청 큰 소리가 한꺼번에 몰려오더라고요.

…그 다음은,<br>몸이 굳어버린 느낌이었어요.

제가 뭘 했는지는 잘 기억이 안 나요.

정신 차려보니....<br>이미 상황은 정리돼 있었고요.

하하…

그래도 결과적으로는 임무도 성공했고,<br>아무도 다치지 않았어요. 

그리고 그때 느꼈죠.

아, 이런 임무는<br>혼자였으면 좀 곤란했을지도 모르겠다고요.

…다행이에요.<br>여기엔 항상 누군가 있으니까.

-> rifleman_talk_topic_select


=== rifleman_talk_topic_3 ===
제 특기요?

아하하… 갑자기 물어보니까 조금 당황스럽네요.

흠....

이 근처만 해도 외계 종족이 정말 많잖아요.

생김새도 다르고, 의사소통 방식도 제각각이고요.

말이 통하는 쪽이 오히려 적은 것 같아요.

그래서요…<br>외계 언어 자격증이 하나 있어요.

2급이요.

아, 오해는 하지 말아주세요!<br>유창하게 말한다거나, 협상까지 할 수 있는 건 아니고요.
대신에,상대가 지금 어떤 상태인지는 어느 정도 알 수 있어요.

지금 호의적인지, 아니면 싸울 생각이 있는지.<br>말보다는 몸짓이나 소리, 반응 같은 걸로요.

물론…<br>100% 맞는 건 아니에요.<br>상황이 급하면 헷갈릴 때도 있고요.<br>그래서 항상 “참고 정도”로만 써야 해요.

그래도 아무것도 모르고 마주치는 것보다는,<br>조금이라도 준비할 수 있잖아요.

전투든, 도망이든, 각오할 수 있으니까요.

* [그녀는 말을 하다 말고, 시선을 피한다.]

에헤헤…<br>그치만, 별거 아니에요.

대단한 능력도 아니고,<br>그냥… 조금 먼저 눈치채는 정도라서요.

-> rifleman_talk_topic_select