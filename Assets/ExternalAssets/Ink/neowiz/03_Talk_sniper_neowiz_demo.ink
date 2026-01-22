VAR DEBUG = false
VAR trust_sniper = 0
VAR first_meet_check = false

저 앞에 저격병이 보인다. 말을 걸어볼까?

+ [좋아]
    말을 걸어봐야 겠다.
    { first_meet_check:
        -> sniper_talk_start
    - else:
        -> sniper_talk_first
    }

+ [아니]
    나중에 이야기하자.
    -> END


=== sniper_talk_first ===
~ first_meet_check = true

아 씨-발 뭐야?!
...아.
너구나.
이번에 새로 왔다는 함장.

* [그녀는 위아래로 한 번 훑어본다.]
- 하. 이 타이밍에 오네, 진짜.

* [잠깐 시선을 피했다가 다시 마주친다.]

뭐…
괜히 기대하진 마라.

네가 뭘 하든
내 일만 방해 안 하면 된다.


-> sniper_talk_topic_select


=== sniper_talk_start ===
{DEBUG:
(DEBUG) trust_sniper = {trust_sniper}
}

{
- trust_sniper < 30:
    {~ 뭐냐. | 무슨 일이지? }
- trust_sniper >= 30 && trust_sniper < 50:
    {~ 왔나. | 느려터졌군. }
- else:
    {~ 이제 오는거냐? | 반갑다. }
}

+ [저격병과 대화를 시작한다.]
    -> sniper_talk_topic_select

+ [떠난다.]
    -> END


=== sniper_talk_topic_select ===
그래서. 할 말 있어?

* [이전 함장은 어땠어?]
    -> sniper_talk_topic_1

* [제일 기억에 남는 임무는 뭐였어?]
    -> sniper_talk_topic_2

* [특기는 뭐야?]
    -> sniper_talk_topic_3

+ ->
    (이제 다른 사람과 대화 해보자.)
    -> END


=== sniper_talk_topic_1 ===
이전 함장?

이런 썅, 그걸 왜 묻는데.

그 사람 얘기해서 뭐 달라지는 거라도 있어?

* [그녀는 시선을 돌린 채, 짧게 숨을 쉰다.]

하아....

일 잘했고.<br>괜히 말 많지 않았고.
…이정도면 됐나?

끝이야.<br>더는 할말 없어.

-> sniper_talk_topic_select


=== sniper_talk_topic_2 ===
제일 기억에 남는 임무?

하.<br>그런 게 왜 궁금한데.

…

* [그녀는 잠시 아무 말도 하지 않는다.]

- 이전 함장과 마지막으로 나갔던 임무다.

장소는 외곽.<br>지원도 없었고, 우회로도 없었지.

그래도 조건은 나쁘지 않았어.
거리도 나왔고, 시야도 확보됐고.

쏠 수 있었지.
그래서 쐈다.

망설일 이유는 없었고, 명중했다.
완벽했어.

문제는…
* [그녀는 말을 멈춘다.]
- 그 다음이었다.

내 판단이 틀렸다고는 생각 안 한다.
그 사람도 그렇게 판단했을 거고.

…그만하지.
이 얘긴 여기까지다.

-> sniper_talk_topic_select


=== sniper_talk_topic_3 ===
특기?

하.<br>그런 걸 꼭 말로 해야 하나.
저격이야.<br>그건 이미 알 거고.

중요한 건 얼마나 잘 쏘냐가 아니야.<br>얼마나 멀리서 보느냐지.

가까이 가면 별별 게 다 보이거든.<br>표정, 숨소리, 망설이는 순간 같은 거.

그런 거 보기 시작하면 판단이 흐려져.<br>그래서 난 항상 거리를 두지.

멀리서 보면 간단해진다.<br>적은 적이고, 쏠지 말지 고민할 필요도 없고.

…그게 제일 안전해.<br><br>나한테도.

됐냐?

더 궁금한 거 없으면<br>난 이만 간다.
-> sniper_talk_topic_select
