﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Code.Model.Skills
{
    public class FakeSkillEndTurn : ActiveSkill {
        public FakeSkillEndTurn(Unit unit) : base(-1) {
            name = "End Turn";
            this.unit = unit;
        }
        public FakeSkillEndTurn(FakeSkillEndTurn other) : base(other) { }
        public override Skill Clone() {
            return new FakeSkillEndTurn(this);
        }

        public override bool RequiresAction() {
            return false;
        }

        public override string GetDescription() {
            return string.Format($"End this unit's turn.");
        }
        public override string GetIconID() {
            return "end_turn";
        }

        public override void Resolve(object choice) {
            base.Resolve(choice);
            GameStateManagerScript.instance.GetActiveUnit().EndTurn();
            AfterResolve();
        }
    }
}
