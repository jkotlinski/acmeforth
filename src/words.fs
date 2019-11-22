variable base 10 base !

: chars ;
: align ;
: aligned ;

: if postpone 0branch here 0 , ; immediate
: begin here ; immediate

variable end
create #buffer 80 allot
: <# #buffer end ! ;
: #> 2drop #buffer end @ over - ;
: hold
#buffer dup 1+ end @ #buffer - move
1 end +!  #buffer c! ;
: sign 0< if '-' hold then ;
: ud/mod
>r 0 r@ um/mod r> swap >r um/mod r> ;
: # base @ ud/mod rot
dup $a < if 7 - then $37 + hold ;
: #s # begin 2dup or while # repeat ;

: 2+ 1+ 1+ ;
: cell+ 2+ ;
: 2@ dup cell+ @ swap @ ;
: 2! swap over ! cell+ ! ;
: cells 2* ;
: s>d dup 0< ;
: nip swap drop ;
: min 2dup < if drop else nip then ;
: max 2dup > if drop else nip then ;
: ?dup dup if dup then ;
: case 0 ; immediate
: endcase postpone drop begin ?dup while postpone then repeat ; immediate
: of postpone (of) here 0 , ; immediate
: endof postpone else ; immediate
: value create , does> @ ;
: 0<> 0= 0= ;
: 0> dup 0< 0= swap 0<> and ;
: <> = 0= ;
: buffer: create allot ;
: hex $10 base ! ;
: decimal #10 base ! ;
: true -1 ;
: false 0 ;
: . s>d swap over dabs <# #s rot sign #> type space ;
: u. 0 <# #s #> type space ;
: save-input >in @ 1 ;
: restore-input drop >in ! 0 ;
: .s ." <" depth s>d swap over dabs <# #s rot sign #> type ." > "
depth 1+ 1 ?do depth i - pick . loop cr ;
: .r ( n1 n2 -- )
swap s>d swap over dabs <# #s rot sign #>
rot over - spaces type space ;
: u.r ( u n -- )
swap 0 <# #s #> rot over - spaces type space ;
: pad here $100 + ;
: erase 0 fill ;
: 2over 3 pick 3 pick ;
: 2swap >r rot rot r> rot rot ;
: [ 0 state ! ; immediate
: ] -1 state ! ;
: count dup 1+ swap c@ ;
: /string dup >r - swap r> + swap ;
: abort depth 0 do drop loop quit ;
: \ refill 0= if source nip >in ! then ; immediate
: bl $20 ;
: within over - >r - r> u< ; \ forth-standard.org

( from FIG UK )
: /mod >r s>d r> fm/mod ;
: / /mod nip ;
: mod /mod drop ;
: */mod >r m* r> fm/mod ;
: */ */mod nip ;
: ?negate 0< if negate then ;
: sm/rem 2dup xor >r over >r abs >r dabs r> um/mod swap r> ?negate swap r> ?negate ;

( from forth-standard.org )
: isspace? BL 1+ U< ;
: isnotspace? isspace? 0= ;
: xt-skip >R BEGIN DUP WHILE OVER C@ R@ EXECUTE WHILE 1 /STRING REPEAT THEN R> DROP ;
: parse-name SOURCE >IN @ /STRING ['] isspace? xt-skip OVER >R ['] isnotspace? xt-skip 2DUP 1 MIN + SOURCE DROP - >IN ! DROP R> TUCK - ;
: DEFER CREATE ['] ABORT , DOES> @ EXECUTE ;
: defer! >body ! ;
: defer@ >body @ ;
: ACTION-OF STATE @ IF POSTPONE ['] POSTPONE DEFER@ ELSE ' DEFER@ THEN ; IMMEDIATE
: IS STATE @ IF POSTPONE ['] POSTPONE DEFER! ELSE ' DEFER! THEN ; IMMEDIATE
: HOLDS BEGIN DUP WHILE 1- 2DUP + C@ HOLD REPEAT 2DROP ;

:code	d+	; ( d1 d2 -- d3 )
	clc
	lda	LSB+1,x
	adc	LSB+3,x
	sta	LSB+3,x
	lda	MSB+1,x
	adc	MSB+3,x
	sta	MSB+3,x
	lda	LSB,x
	adc	LSB+2,x
	sta	LSB+2,x
	lda	MSB,x
	adc	MSB+2,x
	sta	MSB+2,x
	inx
	inx
	rts
;code

: accumulate ( +d0 addr digit - +d1 addr )
swap >r swap base @ um* drop
rot base @ um* d+ r> ;
: pet# ( char -- num )
$7f and dup \ lowercase
':' < if '0' else '7' then - ;
: digit? ( char -- flag )
pet# dup 0< 0= swap base @ < and ;
: >number ( ud addr u -- ud addr u )
begin dup 0= if exit then
over c@ digit? while
>r dup c@ pet# accumulate
1+ r> 1- repeat ;

\ ----- C64 primitives below

:code	c@
	lda	LSB,x
	sta	+ + 1
	lda	MSB,x
	sta	+ + 2
+	lda	$cafe
	sta	LSB,x
	lda	#0
	sta	MSB,x
	rts
;code

:code c!
	lda	LSB,x
	sta	+ + 1
	lda	MSB,x
	sta	+ + 2
	lda	LSB+1,x
+	sta	$1234
	inx
	inx
	rts
;code

:code 1+
	inc LSB, x
	bne +
	inc MSB, x
+	rts
;code

:code litc
	dex

	; load IP
	pla
	sta W
	pla
	sta W + 1

	inc W
	bne +
	inc W + 1
+
	; copy literal to stack
	ldy #0
	lda (W), y
	sta LSB, x
	sty MSB, x

	inc W
	bne +
	inc W + 1
+	jmp (W)
;code

:code lit
	dex

	; load IP
	pla
	sta W
	pla
	sta W + 1

	; copy literal to stack
	ldy #1
	lda (W), y
	sta LSB, x
	iny
	lda (W), y
	sta MSB, x

	lda W
	clc
	adc #3
	sta + + 1
	lda W + 1
	adc #0
	sta + + 2
+	jmp $1234
;code

:code (loop)
	stx	W	; x = stack pointer
	tsx

	inc	$103,x	; i++
	bne	+
	inc	$104,x
+
	lda	$103,x	; lsb check
	cmp	$105,x
	beq	.check_msb

.continue_loop
	ldx	W	; restore x
	jmp	%branch%

.check_msb
	lda	$104,x
	cmp	$106,x
	bne	.continue_loop

	pla		; loop done - skip branch address
	clc
	adc	#3
	sta	W2

	pla
	adc	#0
	sta	W2 + 1

	txa		; sp += 6
	clc
	adc	#6
	tax
	txs

	ldx	W	; restore x
	jmp	(W2)
;code

:code 0branch
	inx
	lda	LSB-1, x
	ora	MSB-1, x
	bne	+
	jmp	%branch%
+ 	; skip offset
	pla
	clc
	adc	#2
	bcc	+
	tay
	pla
	adc	#0
	pha
	tya
+	pha
	rts
;code

:code !
	lda LSB, x
	sta W
	lda MSB, x
	sta W + 1

	ldy #0
	lda LSB+1, x
	sta (W), y
	iny
	lda MSB+1, x
	sta (W), y

	inx
	inx
	rts
;code

:code negate
	jsr %invert%
	jmp %1+%
;code

:code 0<
	lda	MSB,x
	and	#$80
	beq	+
	lda	#$ff
+	sta	LSB,x
	sta	MSB,x
	rts
;code

:code dup
	dex
	lda MSB + 1, x
	sta MSB, x
	lda LSB + 1, x
	sta LSB, x
	rts
;code

:code type
-	lda LSB,x
	ora MSB,x
	bne +
	inx
	inx
	rts
+	jsr %over%
	jsr %c@%
	jsr %emit%
	jsr %1%
	jsr %/string%
	jmp -
;code

:code depth
	txa
	eor #$ff
	tay
	iny
	dex
	sty LSB,x
	lda #0
	sta MSB,x
	rts
;code

:code @
	lda LSB,x
	sta W
	lda MSB,x
	sta W+1

	ldy #0
	lda (W),y
	sta LSB,x
	iny
	lda (W),y
	sta MSB,x
	rts
;code

:code =
	ldy #0
	lda LSB, x
	cmp LSB + 1, x
	bne +
	lda MSB, x
	cmp MSB + 1, x
	bne +
	dey
+	inx
	sty MSB, x
	sty LSB, x
	rts
;code

:code (do)
	pla
	sta	W
	pla
	tay

	lda	MSB+1,x
	pha
	lda	LSB+1,x
	pha

	lda	MSB,x
	pha
	lda	LSB,x
	pha

	inx
	inx

	tya
	pha
	lda	W
	pha
	rts
;code

:code	i
	jmp %r@%
;code

:code	j
	txa
	tsx
	ldy	$107,x
	sty	W
	ldy	$108,x
	tax
	dex
	sty	MSB,x
	lda	W
	sta	LSB,x
	rts
;code

:code +
	lda LSB, x
	clc
	adc LSB + 1, x
	sta LSB + 1, x

	lda MSB, x
	adc MSB + 1, x
	sta MSB + 1, x

	inx
	rts
;code

:code 0=
	ldy #0
	lda MSB, x
	bne +
	lda LSB, x
	bne +
	dey
+	sty MSB, x
	sty LSB, x
	rts
;code

:code sliteral
	jsr	%r>%
	jsr	%1+%
	jsr	%dup%
	jsr	%1+%
	jsr	%swap%
	jsr	%c@%
	jsr	%2dup%
	jsr	%+%
	jsr	%1-%
	jsr	%>r%
	rts
;code

:code 1-
	lda LSB, x
	bne +
	dec MSB, x
+	dec LSB, x
	rts
;code

:code 2dup
	jsr %over%
	jmp %over%
;code

:code over
	dex
	lda MSB + 2, x
	sta MSB, x
	lda LSB + 2, x
	sta LSB, x
	rts
;code

:code swap
	ldy MSB, x
	lda MSB + 1, x
	sta MSB, x
	sty MSB + 1, x

	ldy LSB, x
	lda LSB + 1, x
	sta LSB, x
	sty LSB + 1, x
	rts
;code

:code cr
	jsr	%litc%
	!byte	$d
	jmp	%emit%
;code

:code emit
	lda	LSB, x
	inx
	jmp	PUTCHR
;code

:code /string
	jsr %dup%
	jsr %>r%
	jsr %-%
	jsr %swap%
	jsr %r>%
	jsr %+%
	jmp %swap%
;code

:code -
	lda LSB + 1, x
	sec
	sbc LSB, x
	sta LSB + 1, x
	lda MSB + 1, x
	sbc MSB, x
	sta MSB + 1, x
	inx
	rts
;code

:code 1
	lda	#1
	ldy	#0
	jmp	%pushya%
;code

:code pushya
	dex
	sta	LSB, x
	sty	MSB, x
	rts
;code

:code invert
	lda MSB, x
	eor #$ff
	sta MSB, x
	lda LSB, x
	eor #$ff
	sta LSB,x
	rts
;code

:code branch
	pla
	sta W
	pla
	sta W + 1

	ldy #2
	lda (W), y
	sta + + 2
	dey
	lda (W), y
	sta + + 1
+	jmp $1234
;code

:code	dabs
	jsr	%dup%
	jmp	%?dnegate%
;code

:code	?dnegate
	jsr	%0<%
	inx
	lda	MSB-1,x
	beq	+
	jsr	%dnegate%
+	rts
;code

:code	dnegate
	jsr	%invert%
	jsr	%>r%
	jsr	%invert%
	jsr	%r>%
	jsr	%1%
	jmp	%m+%
;code

:code	m+
	ldy #0
	lda MSB,x
	bpl +
	dey
+	clc
	lda LSB,x
	adc LSB+2,x
	sta LSB+2,x
	lda MSB,x
	adc MSB+2,x
	sta MSB+2,x
	tya
	adc LSB+1,x
	sta LSB+1,x
	tya
	adc MSB+1,x
	sta MSB+1,x
	inx
	rts
;code

:code	+!
	lda	LSB,x
	sta	W
	lda	MSB,x
	sta	W+1

	ldy	#0
	clc

	lda	(W),y
	adc	LSB+1,x
	sta	(W),y
	iny
	lda	(W),y
	adc	MSB+1,x
	sta	(W),y
	inx
	inx
	rts
;code

:code	2*
	asl	LSB, x
	rol	MSB, x
	rts
;code

:code	2/
	lda	MSB,x
	cmp	#$80
	ror	MSB,x
	ror	LSB,x
	rts
;code

:code	and
	lda	MSB, x
	and	MSB + 1, x
	sta	MSB + 1, x

	lda	LSB, x
	and	LSB + 1, x
	sta	LSB + 1, x

	inx
	rts
;code

:code	r>	; must be called using jsr
	pla
	sta W
	pla
	sta W+1
	inc W
	bne +
	inc W+1
+
	dex
	pla
	sta LSB,x
	pla
	sta MSB,x
	jmp (W)
;code

:code	r@	; must be called using jsr
	txa
	tsx
	ldy $103,x
	sty W
	ldy $104,x
	tax
	dex
	sty MSB,x
	lda W
	sta LSB,x
	rts
;code

:code	>r	; must be called using jsr
	pla
	sta W
	pla
	sta W+1
	inc W
	bne +
	inc W+1
+
	lda MSB,x
	pha
	lda LSB,x
	pha
	inx
	jmp (W)
;code

:code	or
	lda	MSB,x
	ora	MSB+1,x
	sta	MSB+1,x
	lda	LSB,x
	ora	LSB+1,x
	sta	LSB+1,x
	inx
	rts
;code

:code	xor
	lda	MSB,x
	eor	MSB+1,x
	sta	MSB+1,x
	lda	LSB,x
	eor	LSB+1,x
	sta	LSB+1,x
	inx
	rts
;code

:code	lshift
-	dec	LSB,x
	bmi	+
	asl	LSB+1,x
	rol	MSB+1,x
	jmp	-
+	inx
	rts
;code

:code	rshift
-	dec	LSB,x
	bmi	+
	lsr	MSB+1,x
	ror	LSB+1,x
	jmp	-
+	inx
	rts
;code

:code	<
    ldy #0
    sec
    lda LSB+1,x
    sbc LSB,x
    lda MSB+1,x
    sbc MSB,x
    bvc +
    eor #$80
+   bpl +
    dey
+   inx
    sty LSB,x
    sty MSB,x
    rts
;code

:code	>
	jsr	%swap%
	jmp	%<%
;code

:code	u<
    ldy #0
    lda MSB, x
    cmp MSB + 1, x
    bcc .false
    bne .true
    ; ok, msb are equal...
    lda LSB + 1, x
    cmp LSB, x
    bcs .false
.true
    dey
.false
    inx
    sty MSB, x
    sty LSB, x
    rts
;code

:code	pick
    txa
    sta + + 1
    clc
    adc LSB,x
    tax
    inx
    lda LSB,x
    ldy MSB,x
+   ldx #0
    sta LSB,x
    sty MSB,x
    rts
;code

:code	rot
	ldy	MSB+2,x
	lda	MSB+1,x
	sta	MSB+2,x
	lda	MSB,x
	sta	MSB+1,x
	sty	MSB,x
	ldy	LSB+2,x
	lda	LSB+1,x
	sta	LSB+2,x
	lda	LSB,x
	sta	LSB+1,x
	sty	LSB,x
	rts
;code

:code	abs
	lda	MSB,x
	bpl	+
	jmp	%negate%
+	rts
;code

:code	m*
	jsr	%2dup%
	jsr	%xor%
	jsr	%>r%
	jsr	%>r%
	jsr	%abs%
	jsr	%r>%
	jsr	%abs%
	jsr	%um*%
	jsr	%r>%
	jmp	%?dnegate%
;code

:code	um*	; wastes W, W2, y
product = W
    lda #$00
    sta product+2 ; clear upper bits of product
    sta product+3
    ldy #$10 ; set binary count to 16
.shift_r
    lsr MSB + 1, x ; multiplier+1 ; divide multiplier by 2
    ror LSB + 1, x ; multiplier
    bcc rotate_r
    lda product+2 ; get upper half of product and add multiplicand
    clc
    adc LSB, x ; multiplicand
    sta product+2
    lda product+3
    adc MSB, x ; multiplicand+1
rotate_r
    ror ; rotate partial product
    sta product+3
    ror product+2
    ror product+1
    ror product
    dey
    bne .shift_r

    lda product
    sta LSB + 1, x
    lda product + 1
    sta MSB + 1, x
    lda product + 2
    sta LSB, x
    lda product + 3
    sta MSB, x
    rts
;code

:code	*
	jsr	%m*%
	inx
	rts
;code

:code	fm/mod
	jsr	%dup%
	jsr	%>r%
	lda	MSB,x
	bpl	+
	jsr	%negate%
	jsr	%>r%
	jsr	%dnegate%
	jsr	%r>%
+	lda	MSB+1,x
	bpl	+
	jsr	%tuck%
	jsr	%+%
	jsr	%swap%
+	jsr	%um/mod%
	jsr	%r>%
	inx
	lda	MSB-1,x
	bpl	+
	jsr	%swap%
	jsr	%negate%
	jsr	%swap%
+	rts
;code

:code	um/mod
        N = W
        SEC
        LDA     LSB+1,X     ; Subtract hi cell of dividend by
        SBC     LSB,X     ; divisor to see if there's an overflow condition.
        LDA     MSB+1,X
        SBC     MSB,X
        BCS     oflo    ; Branch if /0 or overflow.

        LDA     #17     ; Loop 17x.
        STA     N       ; Use N for loop counter.
loop:   ROL     LSB+2,X     ; Rotate dividend lo cell left one bit.
        ROL     MSB+2,X
        DEC     N       ; Decrement loop counter.
        BEQ     end     ; If we're done, then branch to end.
        ROL     LSB+1,X     ; Otherwise rotate dividend hi cell left one bit.
        ROL     MSB+1,X
        lda     #0
        sta     N+1
        ROL     N+1     ; Rotate the bit carried out of above into N+1.

        SEC
        LDA     LSB+1,X     ; Subtract dividend hi cell minus divisor.
        SBC     LSB,X
        STA     N+2     ; Put result temporarily in N+2 (lo byte)
        LDA     MSB+1,X
        SBC     MSB,X
        TAY             ; and Y (hi byte).
        LDA     N+1     ; Remember now to bring in the bit carried out above.
        SBC     #0
        BCC     loop

        LDA     N+2     ; If that didn't cause a borrow,
        STA     LSB+1,X     ; make the result from above to
        STY     MSB+1,X     ; be the new dividend hi cell
        bcs     loop    ; and then branch up.

oflo:   LDA     #$FF    ; If overflow or /0 condition found,
        STA     LSB+1,X     ; just put FFFF in both the remainder
        STA     MSB+1,X
        STA     LSB+2,X     ; and the quotient.
        STA     MSB+2,X

end:    INX
        jmp %swap%
;code

:code	tuck
	jsr	%swap%
	jmp	%over%
;code

:code	char+
	jmp	%1+%
;code

:code	2@
	jsr	%dup%
	jsr	%2+%
	jsr	%@%
	jsr	%swap%
	jmp	%@%
;code

:code	2!
	jsr	%swap%
	jsr	%over%
	jsr	%!%
	jsr	%2+%
	jmp	%!%
;code

:code	bye
	jmp	BYE
;code

:code	execute
	lda	LSB, x
	sta	W
	lda	MSB, x
	sta	W + 1
	inx
	jmp	(W)
;code

:code	(+loop)
	; r> swap r> 2dup +
	jsr	%r>%
	jsr	%swap%
	jsr	%r>%
	jsr	%2dup%
	jsr	%+%

	; rot 0< if tuck swap else tuck then
	jsr	%rot%
	inx
	lda	MSB-1,x
	bpl	.pl
	jsr	%tuck%
	jsr	%swap%
	jmp	++
.pl	jsr	%tuck%
++
	; r@ 1- -rot within 0= if
	jsr	%r@%
	jsr	%1-%
	jsr	%rot%
	jsr	%rot%
	jsr	%within%

	inx
	lda	MSB-1,x
	bne	+

	; >r >r [ ' branch jmp, ] then
	jsr	%>r%
	jsr	%>r%
	jmp	%branch%
+
	; r> 2drop 2+ >r ;
	jsr	%r>%
	inx
	inx
	jsr	%2+%
	jsr	%>r%
	rts
;code

:code	dodoes
    ; behavior pointer address => W
    pla
    sta W
    pla
    sta W + 1

    inc W
    bne +
    inc W + 1
+

    ; push data pointer to param stack
    dex
    lda W
    clc
    adc #2
    sta LSB,x
    lda W + 1
    adc #0
    sta MSB,x

    ldy #0
    lda (W),y
    sta W2
    iny
    lda (W),y
    sta W2 + 1
    jmp (W2)
;code

:code	move
    jsr %>r%
    jsr %2dup%
    jsr %u<%
    jsr %r>%
    jsr %swap%
    jsr %0branch%
    !word CMOVE
    jmp CMOVE_BACK
CMOVE
    txa
    pha
	jsr cmove_getparams
	ldy #0
	ldx	LEN + 1
	beq	.l2
.l1
	lda	(SRC),y ; copy byte
	sta	(DST),y
	iny
	lda	(SRC),y ; copy byte again, to make it faster
	sta	(DST),y
	iny
	bne .l1
	inc	SRC + 1
	inc DST + 1
	dex ; next 256-byte block
	bne .l1
.l2
	ldx	LEN
	beq cmove_done
.l3
	lda (SRC),y
	sta	(DST),y
	iny
	dex
	bne	.l3
cmove_done
	pla
    clc
	adc #3
	tax
	rts

cmove_getparams:
	lda	LSB, x
	sta	LEN
	lda	MSB, x
	sta	LEN + 1
	lda	LSB + 1, x
	sta	DST
	lda	MSB + 1, x
	sta	DST + 1
	lda	LSB + 2, x
	sta	SRC
	lda	MSB + 2, x
	sta	SRC + 1
	rts

CMOVE_BACK
	txa
	pha
	jsr cmove_getparams
    ; copy downwards. adjusts pointers to the end of memory regions.
    lda SRC + 1
    clc
    adc LEN + 1
    sta SRC + 1
    lda DST + 1
    clc
    adc LEN + 1
    sta DST + 1

    ldy LEN
    bne .entry
    beq .pagesizecopy
.copybyte
    lda (SRC),y
    sta (DST),y
.entry
    dey
    bne .copybyte
    lda (SRC),y
    sta (DST),y
.pagesizecopy
    ldx LEN + 1
    beq cmove_done
.initbase
    dec SRC + 1
    dec DST + 1
    dey
    lda (SRC),y
    sta (DST),y
    dey
.copybytes
    lda (SRC),y
    sta (DST),y
    dey
    lda (SRC),y
    sta (DST),y
    dey
    bne .copybytes
    lda (SRC),y
    sta (DST),y
    dex
    bne .initbase
	jmp cmove_done
;code
